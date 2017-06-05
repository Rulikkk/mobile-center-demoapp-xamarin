using System;
using System.Threading.Tasks;
using Android.Content;
using MobileCenterDemoApp.Droid.Dependencies;
using MobileCenterDemoApp.Interfaces;
using MobileCenterDemoApp.Models;
using Xamarin.Auth;
using Xamarin.Forms;
using MobileCenterDemoApp.Helpers;

[assembly: Dependency(typeof(FacebookLoginAndroid))]
namespace MobileCenterDemoApp.Droid.Dependencies
{
    public class FacebookLoginAndroid : IFacebook
    {
        public event Action<string> OnError;
        private OAuth2Authenticator _oAuth2;

        private Intent _authUi;
        private bool _isComplite;
        private SocialAccount _account;

        public async Task<SocialAccount> Login()
        {
            _oAuth2 = SocialNetworkAuthenticators.FacebookAuth;
            _authUi = (Intent)_oAuth2.GetUI(MainActivity.Activity);

            MainActivity.Activity.StartActivity(_authUi);
            _oAuth2.Completed += async (sender, args) =>
            {
                _account = await SocialNetworkAuthenticators.OnCompliteFacebookAuth(args);
                _isComplite = true;
            };
            _oAuth2.Error += (sender, args) => OnError?.Invoke(args.Message);

            await Task.Run(() =>
            {
                while (!_isComplite)
                    Task.Delay(100);

                _authUi.Dispose();
            });
            
            
            return _account;
        }
    }
}