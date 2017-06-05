using System;
using System.Threading.Tasks;
using MobileCenterDemoApp.iOS.Dependencies;
using MobileCenterDemoApp.Interfaces;
using MobileCenterDemoApp.Models;
using UIKit;
using Xamarin.Auth;
using Xamarin.Forms;
using MobileCenterDemoApp.Helpers;

[assembly: Dependency(typeof(FacebookLoginIOS))]
namespace MobileCenterDemoApp.iOS.Dependencies
{
    public class FacebookLoginIOS : IFacebook
    {
        private bool _isComplite;
        private SocialAccount _account;

        public async Task<SocialAccount> Login()
        {
            _isComplite = false;

            OAuth2Authenticator _oAuth2 = SocialNetworkAuthenticators.FacebookAuth;

            _oAuth2.Completed += async (sender, args) =>
            {
                _account = await SocialNetworkAuthenticators.OnCompliteFacebookAuth(args);
                _isComplite = true;
            };
            _oAuth2.Error += (sender, args) => OnError?.Invoke(args.Message);
            
            using (var window = new UIWindow(UIScreen.MainScreen.Bounds))
            {
                window.RootViewController = (UIViewController)_oAuth2.GetUI();
                window.MakeKeyAndVisible();

                // await user login 
                return await Task.Run(() =>
                {
                    while (!_isComplite)
                        Task.Delay(100);

                    return _account;
                });
            }
        }

        public event Action<string> OnError;
    }
}