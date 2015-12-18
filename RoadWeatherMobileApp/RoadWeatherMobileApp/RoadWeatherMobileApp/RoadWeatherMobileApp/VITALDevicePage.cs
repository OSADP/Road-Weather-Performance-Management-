using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using CITOMobileCommon.VITAL;
using Xamarin.Forms;

namespace RoadWeatherMobileApp
{
    public class VITALDevicePage : ContentPage
    {
        public event EventHandler<String> DeviceSelected;

        public VITALDevicePage()
        {

            VitalProvider vp = new VitalProvider();
            //vp.DeviceFound += Vp_DeviceFound;

            List<VitalDevice> deviceList = VitalProvider.ReturnPairedDevices();

            ListView pairedDeviceListView = new ListView { ItemsSource = deviceList };
            var cell = new DataTemplate(typeof(TextCell));
            cell.SetBinding(TextCell.TextProperty, "DeviceName");
            cell.SetBinding(TextCell.DetailProperty, "MacAddress");
            pairedDeviceListView.ItemTemplate = cell;

            pairedDeviceListView.ItemSelected += DeviceListViewOnItemSelected;

            Label titleLabel = new Label
            {
                Text = "RoadWeather - VITAL Integration Demo!",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center
            };

            Button scanButton = new Button
            {
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                Text = "Scan for Devices"
            };
            scanButton.Clicked += ScanButton_Clicked;

            Label pairedListHeaderLabel = new Label
            {
                Text = "Available Paired Devices",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                BackgroundColor = Color.Green,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                XAlign = TextAlignment.Center
            };

            Label unPairedListHeaderLabel = new Label
            {
                Text = "Available Unpaired Devices",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                BackgroundColor = Color.Green,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                XAlign = TextAlignment.Center
            };

            StackLayout mainLayout = new StackLayout
            {
                Children = { titleLabel, pairedListHeaderLabel, pairedDeviceListView}
            };

            Content = mainLayout;

            Padding = new Thickness(0, 20, 0, 0);
        }

        private void DeviceListViewOnItemSelected(object sender, SelectedItemChangedEventArgs selectedItemChangedEventArgs)
        {
            if (selectedItemChangedEventArgs.SelectedItem == null)
            {
                return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
            }
            ((ListView)sender).SelectedItem = null;

            Settings.VitalDeviceMac = ((VitalDevice)selectedItemChangedEventArgs.SelectedItem).MacAddress;

            if (DeviceSelected != null)
                DeviceSelected(this, ((VitalDevice)selectedItemChangedEventArgs.SelectedItem).MacAddress);
        }

        private void ScanButton_Clicked(object sender, EventArgs e)
        {
            IBluetoothHelper btHelper = DependencyService.Get<IBluetoothHelper>();
            btHelper.SearchForNewDevices();

        }
    }
}
