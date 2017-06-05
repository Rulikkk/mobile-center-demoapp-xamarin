namespace MobileCenterDemoApp.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Azure.Mobile.Analytics;
    using MobileCenterDemoApp.Helpers;
    using MobileCenterDemoApp.Models;
    using MobileCenterDemoApp.Services;
    using Xamarin.Forms;
    using MobileCenterDemoApp.Pages;
    using MobileCenterDemoApp.Interfaces;

    public class LoginViewModel : ViewModelBase
    {
        
        private string _errorMessage;

        #region Properties

        /// <summary>
        /// Show header with mobile center logo
        /// </summary>
        public bool ShowHeader
        {
            get
            {
                return string.IsNullOrEmpty(ErrorMessage);
            }
        }

        /// <summary>
        /// Show error image with description
        /// </summary>
        public bool ShowError
        {
            get
            {
                return !string.IsNullOrEmpty(ErrorMessage);
            }
        }
        
        /// <summary>
        /// Error description
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                SetProperty(ref _errorMessage, value);

                // Update show flags
                OnPropertyChanged(nameof(ShowHeader));
                OnPropertyChanged(nameof(ShowError));
            }
        }

        /// <summary>
        /// Button border width
        /// </summary>
        public double BorderWidth { get; }

        #endregion

        /// <summary>
        /// Command for login via Facebook
        /// </summary>
        public Command LoginViaFacebookCommand { get; private set; }

        /// <summary>
        /// Command for login via Twitter
        /// </summary>
        public Command LoginViaTwitterCommand { get; private set; }

        public LoginViewModel()
        {
            Title = "Count my steps";

            LoginViaFacebookCommand = new Command(LoginViaFacebook);
            LoginViaTwitterCommand = new Command(LoginViaTwitter);

            BorderWidth = PlatformSizes.ButtonBorderRadius;

            if(Device.RuntimePlatform == Device.iOS)
            {
                BorderWidth = 23;
            }
        }

        #region Auth

        /// <summary>
        /// Login in Facebook (LoginViaFacebookCommand handler)
        /// </summary>
        private async void LoginViaFacebook()
        {
            Analytics.TrackEvent("Facebook login button clicked",
                new Dictionary<string, string>
                {
                    {"Page", "Login"},
                    {"Category", "Clicks"}
                });

            IFacebook facebookService = DependencyService.Get<IFacebook>(DependencyFetchTarget.GlobalInstance);

            facebookService.OnError += error => AuthError("Facebook", error);

            SocialAccount account = await facebookService.Login();

            Login(account, "Facebook");
        }

        /// <summary>
        /// Login in Twitter (LoginViaTwitterCommand handler)
        /// </summary>
        private async void LoginViaTwitter()
        {
            Analytics.TrackEvent("Twitter login button clicked",
                new Dictionary<string, string>
                {
                    {"Page", "Login"},
                    {"Category", "Clicks"}
                });

            ITwitter twitterService = DependencyService.Get<ITwitter>(DependencyFetchTarget.GlobalInstance);

            twitterService.OnError += error => AuthError("Twitter", error);

            SocialAccount account = await twitterService.Login();

            Login(account, "Twitter");
        }

        private void Login(SocialAccount account, string socialNet)
        {
            Analytics.TrackEvent("Trying to login in Facebook/Twitter",
                new Dictionary<string, string>
                {
                    {"Page", "Login"},
                    {"Category", "Request"},
                    {"API", "Social network"},
                    {"Social network", socialNet},
                    {"Result", (account != null).ToString()},
                    {"Error message", account == null ? "Cancel by user" : string.Empty }
                });

            if (account == null)
            {
                ErrorMessage = "Login failed, please try again";
                return;
            }

            DataStore.Account = account;

            #region Init and retrive data from Google Fit / HealthKit

            string error = string.Empty;
            bool success;

            Action<string> ErrorHandle =
                (errorMessage) =>
            {
                success = false;
                error = errorMessage;
            };

            try
            {
                DataStore.FitnessTracker.OnError += ErrorHandle;                
                if (!DataStore.FitnessTracker.IsConnected)
                {
                    success = false;
                    error = $"Connection to {DataStore.FitnessTracker.ApiName} failed";
                }
                else
                {
                    DataStore.ReadTodayInformation();
                    success = true;
                }
                DataStore.FitnessTracker.OnError -= ErrorHandle;
            }
            catch (Exception e)
            {
                success = false;
                error = e.Message;
            }

            string fitnessApi = Device.RuntimePlatform == Device.Android ? "Google fit" : "HealthKit";

            Analytics.TrackEvent("Trying to retrieve data from HealthKit/Google Fit API.",
                new Dictionary<string, string>
                {
                    {"Page", "Login"},
                    {"Category", "Request"},
                    { "API", DataStore.FitnessTracker?.ApiName ?? fitnessApi },
                    {"Result", success.ToString()},
                    {"Error_message", error}
                });

            #endregion

            App.SwitchMainPage(new MainPage(error));
        }

        /// <summary>
        /// Send information to Mobile center if login failure
        /// </summary>
        /// <param name="socialNet"></param>
        /// <param name="message"></param>
        private void AuthError(string socialNet, string message)
        {
            Analytics.TrackEvent("Trying to login in Facebook/Twitter",
                new Dictionary<string, string>
                {
                    {"Page", "Login"},
                    {"Category", "Request"},
                    {"API", "Social network"},
                    {"Social network", socialNet},
                    {"Result", false.ToString()},
                    {"Error message", message}
                }
            );
        }

        #endregion
    }
}