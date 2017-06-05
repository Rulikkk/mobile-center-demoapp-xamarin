using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileCenterDemoApp.Pages
{
    /// <summary>
    /// Page for showing some error messages
    /// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ErrorPage : ContentPage
    {
        public bool ShowHomePage;

        /// <summary>
        /// Page show Error message
        /// </summary>
        /// <param name="message">Error message</param>
	    public ErrorPage(string message)
        {
            InitializeComponent();
            ErrorLabel.Text = message;
        }

        /// <summary>
        /// Home button click handler
        /// </summary>
        /// <param name="sender">Objects sender</param>
        /// <param name="e">Click arguments</param>
	    private async void HomeClicked(object sender, EventArgs e)
        {
            ShowHomePage = true;
            await Navigation.PopModalAsync();
        }

        /// <summary>
        /// Statistics button click handler
        /// </summary>
        /// <param name="sender">Objects sender</param>
        /// <param name="e">Click arguments</param>
	    private async void StatisticsClicked(object sender, EventArgs e)
        {
            ShowHomePage = false;
            await Navigation.PopModalAsync();
        }
    }
}
