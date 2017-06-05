namespace MobileCenterDemoApp.Pages
{
    using MobileCenterDemoApp.Helpers;
    using MobileCenterDemoApp.Services;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    /// <summary>
    /// Tabbed page
    ///     iOS - default tabbed page
    ///     Android - bottom tabbed page
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class MainPage : BottomTabbedPage
    {
        /// <summary>
        /// User profile page with username and today's statistics
        /// </summary>
        private Page _profilePage;

        /// <summary>
        /// Statistics page with charts for last 5 days
        /// </summary>
        private Page _statisticsPage;

        /// <summary>
        /// Error message from Login page if error exists
        /// </summary>
        private string _errorMessage;

        /// <summary>
        /// Main tabbed page
        /// </summary>
        /// <param name="errorMessage">Error message if exists</param>
        public MainPage(string errorMessage = "")
        {
            InitComponents();
            _errorMessage = errorMessage;
            DataStore.FitnessTracker.OnError += async (obj) => {
                var errorPage = new ErrorPage(obj);
                await Navigation.PushModalAsync(errorPage);
            };
        }
        private void InitComponents()
        {
            _profilePage = new ProfilePage();
            _statisticsPage = new StatisticsPage();

            Children.Add(_profilePage);
            Children.Add(_statisticsPage);

            CurrentPage = _profilePage;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Check Fitness api connection
            if (!DataStore.FitnessTracker.IsConnected)
            {
                await DataStore.FitnessTracker.Connect();
            }

            // If connect read fitness data
            //      if didn't retrive before
            if (DataStore.FitnessTracker.IsConnected)
            {
                _errorMessage = string.Empty;
                DataStore.ReadTodayInformation();
                return;
            }

            if (string.IsNullOrEmpty(_errorMessage))
                return;

            // Show error message
            ErrorPage errorPage = new ErrorPage(_errorMessage);
            await Navigation.PushModalAsync(errorPage);
            _errorMessage = string.Empty;

            if (errorPage.ShowHomePage)
                CurrentPage = _profilePage;
            else
                CurrentPage = _statisticsPage;
        }
    }
}
