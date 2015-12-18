using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace RoadWeatherMobileApp
{
    public class MenuDetailPage : MasterDetailPage
    {
        public MenuDetailPage()
        {
            Master = new ContentPage
            {
                Title = "Menu",
                BackgroundColor = Color.Gray,
                Content = new StackLayout
                {
                    Padding = new Thickness(10, 40),
                    Children = { Link("Home"), Link("VITAL") }
                    //, Link("Debug"), Link("Logout"), VersionLabel("Version: " + NotificationProvider.GetApplicationVersion()) }
                }
            };

            Detail = new NavigationPage(new HomePage());
        }

        private Label VersionLabel(string versionString)
        {
            var label = new Label
            {
                Text = versionString,
                TextColor = Color.Black,
                FontSize = 16
            };
            return label;
        }

        private Button Link(string name)
        {
            var button = new Button
            {
                Text = name,
                TextColor = Color.White,
                BackgroundColor = Constents.Color_CITO_Aqua,
                FontSize = 18
            };
            button.Clicked += delegate {

                if (name == "Home")
                {
                    this.Detail = new NavigationPage(new HomePage());
                }
                else if (name == "VITAL")
                {
                    VITALDevicePage vd = new VITALDevicePage();
                    vd.DeviceSelected += Vd_DeviceSelected;
                    this.Detail = new NavigationPage(vd);
                }
                /*else if (name == "Logout")
                {
                    //App.ClearCredentials();
                    MDPage.Detail = new NavigationPage(new HomePage());
                }
                */
                this.IsPresented = false;
            };
            return button;
        }

        private void Vd_DeviceSelected(object sender, string e)
        {
            this.Detail = new NavigationPage(new HomePage());
        }
    }
}
