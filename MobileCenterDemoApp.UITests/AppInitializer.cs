using System;
using System.IO;
using System.Linq;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace MobileCenterDemoApp.UITests
{
    public class AppInitializer
    {
        public static IApp StartApp(Platform platform)
        {
            if (platform == Platform.Android)
            {
                              
                return ConfigureApp.Android
                    .InstalledApp("com.mobilecenterdemoapp.xamarin")
                    .StartApp();
            }

            if (platform == Platform.iOS)
            {
                return ConfigureApp
                    .iOS
                    .InstalledApp("com.mobilecenterdemoapp.xamarin")
                    .StartApp();
            }

            throw new PlatformNotSupportedException();
        }
    }
}

