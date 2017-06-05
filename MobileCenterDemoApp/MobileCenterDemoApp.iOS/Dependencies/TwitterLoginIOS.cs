using System;
using System.Threading.Tasks;
using MobileCenterDemoApp.Interfaces;
using MobileCenterDemoApp.Models;
using UIKit;
using Xamarin.Auth;
using Xamarin.Forms;
using MobileCenterDemoApp.iOS.Dependencies;
using Twitter;
using Social;
using Accounts;

[assembly: Dependency(typeof(TwitterLoginiOS))]
namespace MobileCenterDemoApp.iOS.Dependencies
{
    // ReSharper disable once InconsistentNaming
    public class TwitterLoginiOS : ITwitter
    {
        public event Action<string> OnError;

        private bool _isComplite;

        public async Task<SocialAccount> Login()
        {
            SocialAccount account = null;
  
            _isComplite = false;
            OAuth1Authenticator _oAuth1 = Helpers.SocialNetworkAuthenticators.TwitterAuth;
            UIViewController controller = (UIViewController)_oAuth1.GetUI();

            _oAuth1.Completed += async (sender, args) =>
            {
                account = await Helpers.SocialNetworkAuthenticators.OnCompliteTwitterAuth(args);
                _isComplite = true;
                controller.DismissViewController(true, null);
            };
            _oAuth1.Error += (sender, args) =>
            {
                OnError?.Invoke(args.Message);
                controller.DismissViewController(true, null);
            };

            using (UIWindow window = new UIWindow(UIScreen.MainScreen.Bounds))
            {
                window.RootViewController = controller;
                window.MakeKeyAndVisible();

                // await user login 
                return await Task.Run(() =>
                {
                    while (!_isComplite)
                        Task.Delay(100);

					return account;
				});
			}
        }
    }
}