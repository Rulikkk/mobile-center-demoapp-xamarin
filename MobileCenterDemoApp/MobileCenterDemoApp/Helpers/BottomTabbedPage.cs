namespace MobileCenterDemoApp.Helpers
{
    /// <summary>
    /// Special class for bottom tabbed page on Android
    ///     on iOS it's standard tabbed page
    /// </summary>
    public class BottomTabbedPage : Xamarin.Forms.TabbedPage
    {
        public enum BarThemeTypes { Light, DarkWithAlpha, DarkWithoutAlpha }

        public bool FixedMode { get; set; }

        public BarThemeTypes BarTheme { get; set; }

        public void RaiseCurrentPageChanged()
        {
            OnCurrentPageChanged();
        }
    }
}
