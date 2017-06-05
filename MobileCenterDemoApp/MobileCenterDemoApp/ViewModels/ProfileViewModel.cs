namespace MobileCenterDemoApp.ViewModels
{
    using MobileCenterDemoApp.Helpers;
    using MobileCenterDemoApp.Services;
    using Xamarin.Forms;

    public class ProfileViewModel : ViewModelBase
    {
        /// <summary>
        /// User name from Social net
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// Today steps count
        /// </summary>
        public int StepsCount
        {
            get
            {
                return DataStore.TodaySteps;
            }
        }

        /// <summary>
        /// Today calories count
        /// </summary>
        public double Calories
        {
            get
            {
                return DataStore.TodayCalories;
            }
        }

        /// <summary>
        /// Today distance (km)
        /// </summary>
        public double Distance
        {
            get
            {
                return DataStore.TodayDistance;
            }
        }

        /// <summary>
        /// Today activity time
        /// </summary>
        public string Time
        {
            get
            {
                return $"{DataStore.TodayActiveTime:%h}h {DataStore.TodayActiveTime:%m}m";
            }
        }

        /// <summary>
        /// Account photo image source
        /// </summary>
        public ImageSource AccountImageSource { get; }

        public ProfileViewModel()
        {
            Username = DataStore.Account?.UserName;
            AccountImageSource = DataStore.Account?.ImageSource;
            DataStore.DataFill += SetProperties;
        }

        /// <summary>
        /// Update properties on view
        /// </summary>
        private void SetProperties()
        {
            OnPropertyChanged(nameof(StepsCount));
            OnPropertyChanged(nameof(Calories));
            OnPropertyChanged(nameof(Distance));
            OnPropertyChanged(nameof(Time));
        }
    }
}