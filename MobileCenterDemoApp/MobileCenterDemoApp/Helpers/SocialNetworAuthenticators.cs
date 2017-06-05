using MobileCenterDemoApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;

namespace MobileCenterDemoApp.Helpers
{
    public static class SocialNetworkAuthenticators
    {
        /// <summary>
        /// OAuth2 authenticator for Facebook login
        /// </summary>
        public static OAuth2Authenticator FacebookAuth => new OAuth2Authenticator(
            clientId: KeysAndSecrets.FacebookClientId,
            scope: "public_profile",
            authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),
            redirectUrl: new Uri("https://localhost/facebook"))
        {
            ClearCookiesBeforeLogin = false,
            AllowCancel = true,
            Title = "Mobile Center",
            ShowUIErrors = false
        };

        /// <summary>
        /// OAuth1 authenticator for Twitter login
        /// </summary>
        public static OAuth1Authenticator TwitterAuth => new OAuth1Authenticator(
            consumerKey: KeysAndSecrets.TwitterConsumerKey,
            consumerSecret: KeysAndSecrets.TwitterConsumerSecret,
            requestTokenUrl: new Uri("https://api.twitter.com/oauth/request_token"),
            authorizeUrl: new Uri("https://api.twitter.com/oauth/authorize"),
            accessTokenUrl: new Uri("https://api.twitter.com/oauth/access_token"),
            callbackUrl: new Uri("http://mobile.twitter.com")
        )
        {
            ClearCookiesBeforeLogin = false,
            AllowCancel = true,
            Title = "Mobile Center",
            ShowUIErrors = false
        };
      
        /// <summary>
        /// Event handler for OAuth2Authenticator.OnComplete
        /// </summary>
        /// <param name="args"></param>
        /// <returns>Social account with name, user id and account image</returns>
        public static async Task<SocialAccount> OnCompliteFacebookAuth(AuthenticatorCompletedEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (!args.IsAuthenticated)
            {
                return null;
            }

            const string FacebookGraphUrl = "https://graph.facebook.com";

            #region Get username and user id from facebook

            OAuth2Request request = new OAuth2Request("GET", new Uri($"{FacebookGraphUrl}/me"), null, args.Account);
            Response response = await request.GetResponseAsync();
            string text = response.GetResponseText();
            Dictionary<string, string> deserializeObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);

            SocialAccount account = new SocialAccount();
            string name;
            if (deserializeObject.TryGetValue("name", out name))
                account.UserName = name;

            string id;
            if (deserializeObject.TryGetValue("id", out id))
                account.UserId = id;

            #endregion get username and user id

            #region Get account photo 

            request = new OAuth2Request("GET", new Uri($"{FacebookGraphUrl}/v2.9/{account.UserId}/picture"),
                new Dictionary<string, string>
                {
                        {"height", 400.ToString() },
                        {"width", 400.ToString() }
                }, args.Account);
            response = await request.GetResponseAsync();
            account.ImageSource = ImageSource.FromStream(response.GetResponseStream);

            #endregion Get account photo 

            return account;
        }

        public static async Task<SocialAccount> OnCompliteTwitterAuth(AuthenticatorCompletedEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (!args.IsAuthenticated)
            {
                return null;
            }

            const string twitterVerifyCredentials = "https://api.twitter.com/1.1/account/verify_credentials.json";

            // Get user id and User name
            SocialAccount account = new SocialAccount
            {
                UserId = args.Account.Properties["user_id"],
                UserName = args.Account.Properties["screen_name"]
            };

            // Get url to account image
            OAuth1Request request = new OAuth1Request("GET",
                new Uri(twitterVerifyCredentials),
                null, args.Account);
            Response response = await request.GetResponseAsync();

            JObject jsonObject = JObject.Parse(response.GetResponseText());

            string uri = ((string)jsonObject["profile_image_url"])      // Get url for response
                .Replace("_normal", "_400x400");                        // Change resolution of image 
            account.ImageSource = ImageSource.FromUri(new Uri(uri));

            return account;
        }
    }
}
