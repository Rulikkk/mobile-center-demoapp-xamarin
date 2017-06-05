namespace MobileCenterDemoApp.Helpers
{
    using Xamarin.Forms;

    /// <summary>
    /// Contains consts specific for each platform
    /// </summary>
    public static class PlatformSizes
    {
        /// <summary>
        /// Border radius for Round buttons
        /// </summary>
        public static double ButtonBorderRadius { get; }


        static PlatformSizes()
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                ButtonBorderRadius = 100;
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                ButtonBorderRadius = 23;
            }
        }
    }
}
