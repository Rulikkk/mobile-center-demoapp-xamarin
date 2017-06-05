using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using MobileCenterDemoApp.Helpers;
using MobileCenterDemoApp.Services;
using MobileCenterDemoApp.Pages;
using Xamarin.Forms;

namespace MobileCenterDemoApp
{
    public partial class App : Application
    {
        private static bool _alreadyInit = false;

        public App()
        {
            InitializeComponent();

            if (_alreadyInit)
            {
                MainPage = DataStore.Account != null
                    ? (Page)new MainPage()
                    : (Page)new LoginPage();
            }
            else
            {
                MobileCenter.Start($"ios={KeysAndSecrets.MobileCenterAppKeyForIos};android={KeysAndSecrets.MobileCenterAppKeyForAndroid}", typeof(Analytics), typeof(Crashes));
                
                MainPage = new LoginPage();

                _alreadyInit = true;

            }
        }

        /// <summary>
        /// Switch app main page
        /// </summary>
        /// <param name="page">New main page</param>
        public static void SwitchMainPage(Page page)
        {
            Current.MainPage = page;
        }

        protected override async void OnResume()
        {
            if (!DataStore.FitnessTracker.IsConnected)
                await DataStore.FitnessTracker.Connect();

            base.OnResume();
        }

        protected override async void OnStart()
        {
            if (!DataStore.FitnessTracker.IsConnected)
                await DataStore.FitnessTracker.Connect();

            base.OnStart();
        }

        protected override void OnSleep()
        {
            if (DataStore.FitnessTracker.IsConnected)
                DataStore.FitnessTracker.Disconnect();

            base.OnSleep();
        }
    }
}
