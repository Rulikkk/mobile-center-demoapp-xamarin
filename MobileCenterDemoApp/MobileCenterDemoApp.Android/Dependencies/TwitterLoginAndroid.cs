using System;
using System.Threading.Tasks;
using Android.Content;
using MobileCenterDemoApp.Droid.Dependencies;
using MobileCenterDemoApp.Interfaces;
using MobileCenterDemoApp.Models;
using Xamarin.Auth;
using Xamarin.Forms;

[assembly: Dependency(typeof(TwitterLoginAndroid))]
namespace MobileCenterDemoApp.Droid.Dependencies
{
    public class TwitterLoginAndroid : ITwitter
    {
        public event Action<string> OnError;
        private OAuth1Authenticator _oAuth1;
        private Intent _authUi;
        private bool _isComplite;

        public async Task<SocialAccount> Login()
        {
            _oAuth1 = Helpers.SocialNetworkAuthenticators.TwitterAuth;            
            
            SocialAccount account = null;
            _oAuth1.Completed += async (sender, args) =>
            {
                if(args.IsAuthenticated)
                    account = await Helpers.SocialNetworkAuthenticators.OnCompliteTwitterAuth(args);
                _isComplite = true;
            };
            _oAuth1.Error += (sender, args) => OnError?.Invoke(args.Message);
     
            _authUi = (Intent)_oAuth1.GetUI(MainActivity.Activity);
            MainActivity.Activity.StartActivityForResult(_authUi, -1);

            return await Task.Run(() =>
            {
                while (!_isComplite)
                    Task.Delay(100);
                
                _authUi.Dispose();

                return account;
            });
        }

    }
}