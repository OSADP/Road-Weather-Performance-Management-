using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using CITOMobileCommon.Locations;
using CITOMobileCommon.VITAL;


namespace RoadWeatherMobileApp
{
    public class App : Application
    {
        private DateTime lastResume;
        private DateTime lastPause;
        private const int MIN_RESUME_DIFF_MS = 5000;

        static MasterDetailPage MDPage;

        public App()
        {
            MDPage = new MenuDetailPage();
            MainPage = MDPage;
        }

        

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            if ((DateTime.Now - lastPause).TotalMilliseconds > MIN_RESUME_DIFF_MS)
            {
                MessagingCenter.Send<App>(this, "OnSleep");
                lastPause = DateTime.Now;
            }
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            if ((DateTime.Now - lastResume).TotalMilliseconds > MIN_RESUME_DIFF_MS)
            {
                MessagingCenter.Send<App>(this, "OnResume");
                lastResume = DateTime.Now;
            }
        }
    }
}
