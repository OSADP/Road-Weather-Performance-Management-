using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Acr.UserDialogs;
using CITOMobileCommon;
using CITOMobileCommon.VITAL;
using CITOMobileCommon.Locations;
using CITOMobileCommon.VITAL.Commands;
using CITOMobileCommon.VITAL.Responses;
using System.Diagnostics;
using CITOMobileCommon.Models;

namespace RoadWeatherMobileApp
{
    public class HomePage : ContentPage
    {
        private MotoristAlertsService MotoristAlertSvc;
        Label AlertLabel;
        Image AlertImage;
        Label ErrorStatus;
        Label CloudStatus;
        Label GPSStatus;
        Label VitalStatus;

        private LocationMonitor LocMonitor;
        private VitalDevice CurrentVitalDevice;

        public HomePage()
        {
            MotoristAlertSvc = new MotoristAlertsService(Constents.BASE_API_URL);
            LocMonitor = new LocationMonitor(Constents.BASE_API_URL);

            AlertLabel = new Label
            {
                Text = "",
                TextColor = Color.Red,
                FontSize = 40,
                HorizontalOptions = LayoutOptions.Center
            };

            AlertImage = new Image
            {
                Aspect = Aspect.AspectFit,
                WidthRequest = 190,
                HeightRequest = 190
            };
            AlertImage.Source = "";

            ErrorStatus = new Label
            {
                Text = "Trying To Connect To Vital",
                TextColor = Color.Gray,
                FontSize = 15
            };

            CloudStatus = new Label
            {
                Text = "Cloud",
                TextColor = Color.Gray,

                FontSize = 15
            };

            GPSStatus = new Label
            {
                Text = "GPS",
                TextColor = Color.Gray,
                FontSize = 15
            };

            VitalStatus = new Label
            {
                Text = "VITAL",
                TextColor = Color.Gray,
                FontSize = 15
            };

            StackLayout statusStackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 10,
                Padding = 10,
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.End,
                Children = { ErrorStatus, CloudStatus, GPSStatus, VitalStatus }
            };

            StackLayout alertStackLayout = new StackLayout
            {
                Spacing = 10,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Children = { AlertImage, AlertLabel }
            };

            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.Fill,
                Children = { alertStackLayout, statusStackLayout }
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            LocMonitor.CloudActivityEvent += LocMonitor_CloudActivityEvent;
            LocMonitor.LocationActivityEvent += LocMonitor_LocationActivityEvent;
            LocMonitor.VitalActivityEvent += LocMonitor_VitalActivityEvent;
            LocMonitor.StatusEvent += LocMonitor_StatusEvent;

            ConnectToVITAL();
        }

        private void LocMonitor_StatusEvent(string eventString)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ErrorStatus.Text = eventString;

                ClearErrorStatus(3000);
            });
        }

        private void RetryConnectToVITAL()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                for (int i = 20; i > 0; i--)
                {
                    ErrorStatus.Text = "Retry VITAL Connection in " + i.ToString() + " sec";
                    await Sleep(1000);
                }
                ConnectToVITAL();
            });
        }

        private void ClearErrorStatus(int msDelay)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                
                await Sleep(msDelay);
                ErrorStatus.Text = "";

            });
        }

        private void ConnectToVITAL()
        {
            if (!String.IsNullOrEmpty(Settings.VitalDeviceMac))
            {
                ErrorStatus.Text = "Finding Saved VITAL Device";
                var selectedVitalDeviceMac = Settings.VitalDeviceMac;

                VitalProvider vp = new VitalProvider();

                CurrentVitalDevice = null;

                List<VitalDevice> deviceList = VitalProvider.ReturnPairedDevices();

                Boolean vitalDeviceFound = false;
                foreach (VitalDevice vd in deviceList)
                {
                    if (vd.MacAddress == selectedVitalDeviceMac)
                    {
                        TryToConnectToVitalDevice(vd);
                        vitalDeviceFound = true;
                    }
                }

                if(!vitalDeviceFound)
                {
                    ErrorStatus.Text = "Selected Vital Device Not Found";
                    RetryConnectToVITAL();
                }
            }
            else
            {
                ErrorStatus.Text = "No Vital Device Selected";
                this.LocMonitor.StartMonitoringLocation(true);
                MotoristAlertSvc.StartService(this.LocMonitor);
                MotoristAlertSvc.NewMotoristAlertEvent += MotoristAlertSvc_NewMotoristAlertEvent;
            }
        }

        private async Task Sleep(int ms)
        {
            await Task.Delay(ms);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            this.LocMonitor.StopMonitoringLocation();
            this.LocMonitor.DisconnectCurrentVitalDevice();
            CurrentVitalDevice = null;

            LocMonitor.CloudActivityEvent -= LocMonitor_CloudActivityEvent;
            LocMonitor.LocationActivityEvent -= LocMonitor_LocationActivityEvent;
            LocMonitor.VitalActivityEvent -= LocMonitor_VitalActivityEvent;
            LocMonitor.StatusEvent -= LocMonitor_StatusEvent;

            MotoristAlertSvc.NewMotoristAlertEvent -= MotoristAlertSvc_NewMotoristAlertEvent;
            MotoristAlertSvc.StopService();
        }

        private void LocMonitor_LocationActivityEvent(string sender, bool connected, bool sending)
        {
            SetStatusLabelColor(GPSStatus, connected, sending);
        }

        private void LocMonitor_VitalActivityEvent(string sender, bool connected, bool sending)
        {
            SetStatusLabelColor(VitalStatus, connected, sending);
        }

        private void LocMonitor_CloudActivityEvent(string sender, bool connected, bool sending)
        {
            SetStatusLabelColor(CloudStatus, connected, sending);
        }
        private void SetStatusLabelColor(Label label, bool connected, bool sending)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (connected)
                {
                    if (label.TextColor != Color.Lime)
                        label.TextColor = Color.Lime;
                }
                else
                {
                    if (label.TextColor != Color.Red)
                        label.TextColor = Color.Red;
                }
            });
        }

        private void TryToConnectToVitalDevice(VitalDevice vd)
        {
            if (!this.LocMonitor.IsConnectedToVital())
            {
                Task.Factory.StartNew(() =>
               {
                   if (vd.Connect())
                   {
                       Device.BeginInvokeOnMainThread(() =>
                       {
                           ErrorStatus.Text = "Connected To VITAL";
                       });

                       CurrentVitalDevice = vd;

                       MotoristAlertSvc.StartService(this.LocMonitor);
                       MotoristAlertSvc.NewMotoristAlertEvent += MotoristAlertSvc_NewMotoristAlertEvent;

                       this.LocMonitor.AddVitalInformation(CurrentVitalDevice);

                       ClearErrorStatus(2000);
                   }
                   else
                   {
                       Device.BeginInvokeOnMainThread(() =>
                       {
                           ErrorStatus.Text = "VITAL Connection Failed";
                       });

                       RetryConnectToVITAL();
                   }
               });
            }
            else
            {
                ErrorStatus.Text = "VITAL Already Conencted";
            }
        }

        private void MotoristAlertSvc_NewMotoristAlertEvent(CITOMobileCommon.Models.MotoristAlerts newMotoristAlerts)
        {

            Device.BeginInvokeOnMainThread(() =>
            {
                String imgSrc = null;
                String alertText = null;
                //Check for PikAlert with ICE, PikAlert Visibility, then Queue, then Speed Harm, then PikAlert PrecipAlert
                if (newMotoristAlerts.Pik.Count > 0)
                {

                    if (newMotoristAlerts.Pik[0].PavementAlert.ToLower().Contains("icy") ||
                    newMotoristAlerts.Pik[0].PavementAlert.ToLower().Contains("ice"))
                    {
                        //icy
                        imgSrc = "icy.png";
                        alertText = "Caution! Icy Road";
                    }
                    else if (newMotoristAlerts.Pik[0].PavementAlert.ToLower().Contains("snowy") ||
                    newMotoristAlerts.Pik[0].PavementAlert.ToLower().Contains("snow"))
                    {
                        //snowy
                        imgSrc = "slippery.png";
                        alertText = "Caution! Snow Covered Road";
                    }
                    else if (newMotoristAlerts.Pik[0].PavementAlert.ToLower().Contains("wet"))
                    {
                        //wet
                        imgSrc = "slippery.png";
                        alertText = "Caution! Wet Road";
                    }
                    else if (newMotoristAlerts.Pik[0].VisibilityAlert.ToLower().Contains("fog"))
                    {
                        imgSrc = "fog.png";
                        alertText = "Caution! Fog";
                    }
                }

                if(String.IsNullOrEmpty(imgSrc))
                {
                    if (newMotoristAlerts.QWarn.Count > 0)
                    {
                        imgSrc = "alert.png";
                        string infloAlert = "Traffic Backup in " + newMotoristAlerts.QWarn[0].DistanceToQueue + " of a Mile ";
                        alertText = infloAlert;
                    }
                    else
                    {
                        if (newMotoristAlerts.SpdHarm.Count > 0)
                        {
                            imgSrc = GetSpeedHarmImage(newMotoristAlerts.SpdHarm[0]);
                            string speedinfloAlert = newMotoristAlerts.SpdHarm[0].RoadwayId + " at Mile " + newMotoristAlerts.SpdHarm[0].BeginMM;
                            alertText = speedinfloAlert;
                        }
                    }
                }
                

                if(!String.IsNullOrEmpty(imgSrc))
                {
                    AlertImage.Source = imgSrc;
                }
                else
                {
                    AlertImage.Source = "";
                }

                if(!String.IsNullOrEmpty(alertText))
                {
                    AlertLabel.Text = alertText;
                }
                else
                {
                    AlertLabel.Text = "";
                }
            });
        }

        private String GetSpeedHarmImage(MotoristAlerts.SpdHarmAlert alert)
        {
            String img = null;

            if(alert.RecommendedSpeed >=55 && alert.RecommendedSpeed < 60)
            {
                img= "speed_harm_55.png";
            }
            else if (alert.RecommendedSpeed >= 50 && alert.RecommendedSpeed < 55)
            {
                img = "speed_harm_50.png";
            }
            else if (alert.RecommendedSpeed >= 45 && alert.RecommendedSpeed < 50)
            {
                img = "speed_harm_45.png";
            }
            else if (alert.RecommendedSpeed >= 40 && alert.RecommendedSpeed < 45)
            {
                img = "speed_harm_40.png";
            }
            else if (alert.RecommendedSpeed >= 35 && alert.RecommendedSpeed < 40)
            {
                img = "speed_harm_35.png";
            }
            else if (alert.RecommendedSpeed >= 30 && alert.RecommendedSpeed < 35)
            {
                img = "speed_harm_30.png";
            }
            else if (alert.RecommendedSpeed >= 25 && alert.RecommendedSpeed < 30)
            {
                img = "speed_harm_25.png";
            }
            else if (alert.RecommendedSpeed >= 20 && alert.RecommendedSpeed < 25)
            {
                img = "speed_harm_20.png";
            }
            else if (alert.RecommendedSpeed >= 15 && alert.RecommendedSpeed < 20)
            {
                img = "speed_harm_15.png";
            }
            else if (alert.RecommendedSpeed >= 0 && alert.RecommendedSpeed < 15)
            {
                //10 MPH
                img = "speed_harm_10.png";
            }

            return img;
        }
    }
}
