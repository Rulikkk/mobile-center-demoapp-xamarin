namespace MobileCenterDemoApp.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MobileCenterDemoApp.Interfaces;
    using MobileCenterDemoApp.Models;
    using Xamarin.Forms;
    using Helpers;
    using Microsoft.Azure.Mobile.Analytics;
    using System.Runtime.CompilerServices;

    public static class DataStore
    {
        /// <summary>
        /// User account
        /// </summary>
        public static SocialAccount Account { get; set; }

        #region Lazy instances

        private static Lazy<IFitnessTracker> _fitnessServiceLazy =
            new Lazy<IFitnessTracker>(() => DependencyService.Get<IFitnessTracker>(DependencyFetchTarget.GlobalInstance));

        #endregion

        #region Services

        /// <summary>
        /// Fitness service API
        /// </summary>
        public static IFitnessTracker FitnessTracker => _fitnessServiceLazy.Value;

        #endregion

        #region Fitness data

        /// <summary>
        /// Raise when fitness data updated
        /// </summary>
        public static event Action DataFill;

        /// <summary>
        /// Total today steps 
        /// </summary>
        public static int TodaySteps { get; private set; }

        /// <summary>
        /// Total today calories 
        /// </summary>
        public static int TodayCalories { get; private set; }

        /// <summary>
        /// Total today distance 
        /// </summary>
        public static double TodayDistance { get; private set; }

        /// <summary>
        /// Total today activity time 
        /// </summary>
        public static TimeSpan TodayActiveTime { get; private set; }

        /// <summary>
        /// Steps statistics for last 5 days
        /// </summary>
        public static double[] FiveDaysSteps { get; private set; }

        /// <summary>
        /// Calories statistics for last 5 days
        /// </summary>
        public static double[] FiveDaysCalories { get; private set; }

        /// <summary>
        /// Distance statistics for last 5 days
        /// </summary>
        public static double[] FiveDaysDistance { get; private set; }

        /// <summary>
        /// Activity time statistics for last 5 days
        /// </summary>
        public static TimeSpan[] FiveDaysActiveTime { get; private set; }

        /// <summary>
        /// Retrieving data from API
        /// </summary>
        public static bool StatisticsInit { get; private set; }

        #endregion

        #region Retrieve fitness data

        /// <summary>
        /// Update data from Fitness API
        /// </summary>
        public static void ReadTodayInformation()
        {
            ReadStatisticsInformation();
        }

        private static bool _statisticsInLoad = false;

        /// <summary>
        /// Retrieving data from Fitness API
        /// </summary>
        /// <param name="reload">Reload information</param>       
        public static void ReadStatisticsInformation(bool reload = false)
        {
            if (FitnessTracker == null)
                throw new NullReferenceException(nameof(FitnessTracker));

            if (!FitnessTracker.IsConnected)
                return;

            if (_statisticsInLoad)
                return;

            if (StatisticsInit && !reload)
                return;

            _statisticsInLoad = true;

            DateTime startDate = DateTime.Now.Date.AddDays(-4);
            DateTime endDate = DateTime.Now.Date.AddDays(1.01);

            FitnessTracker.StepsByPeriod(startDate, endDate, (obj) =>
            {
                FiveDaysSteps = obj.Select(Convert.ToDouble).ToArray();
                FinishLoad();
            });
            FitnessTracker.DistanceByPeriod(startDate, endDate, (obj) =>
            {
                FiveDaysDistance = obj.ToArray();
                FinishLoad();
            });
            FitnessTracker.CaloriesByPeriod(startDate, endDate, (obj) =>
            {
                FiveDaysCalories = obj.ToArray();
                FinishLoad();
            });
            FitnessTracker.ActiveTimeByPeriod(startDate, endDate, (obj) =>
            {
                FiveDaysActiveTime = obj.ToArray();
                FinishLoad();
            });
        }

        private static void FinishLoad()
        {
            if (FiveDaysSteps == null
                || FiveDaysCalories == null
                || FiveDaysDistance == null
                || FiveDaysActiveTime == null)
            {
                return;
            }

            FiveDaysSteps = FiveDaysSteps.RemoveLastElements(1).ToArray();
            FiveDaysCalories = FiveDaysCalories.RemoveLastElements(1).ToArray();
            FiveDaysDistance = FiveDaysDistance.RemoveLastElements(1).Select(x => x / 1000).ToArray();
            FiveDaysActiveTime = FiveDaysActiveTime.RemoveLastElements(1).ToArray();

            #region If statistics less than 5 days

            Func<int, IEnumerable<int>> range = i => Enumerable.Range(0, 5 - i);

            if (FiveDaysSteps.Length < 5)
                FiveDaysSteps = range(FiveDaysSteps.Length).Select(x => 0D).Concat(FiveDaysSteps).ToArray();
            if (FiveDaysCalories.Length < 5)
                FiveDaysCalories = range(FiveDaysCalories.Length).Select(x => 0D).Concat(FiveDaysCalories).ToArray();
            if (FiveDaysDistance.Length < 5)
                FiveDaysDistance = range(FiveDaysDistance.Length).Select(x => 0D).Concat(FiveDaysDistance).ToArray();
            if (FiveDaysActiveTime.Length < 5)
                FiveDaysActiveTime = range(FiveDaysActiveTime.Length)
                    .Select(x => TimeSpan.FromMilliseconds(0))
                    .Concat(FiveDaysActiveTime)
                    .ToArray();

            #endregion

            TodaySteps = Convert.ToInt32(Math.Round(FiveDaysSteps.Last()));
            TodayCalories = Convert.ToInt32(Math.Round(FiveDaysCalories.Last()));
            TodayDistance = Math.Round(FiveDaysDistance.Last(), 2);
            TodayActiveTime = FiveDaysActiveTime.Last();

            _statisticsInLoad = false;

            StatisticsInit = true;

            DataFill?.Invoke();

            Analytics.TrackEvent("Trying to retrieve data from HealthKit/Google Fit API.",
         new Dictionary<string, string>
         {
                    {"Page", "Login"},
                    {"Category", "Request"},
                    {"API", FitnessTracker?.ApiName },
                    {"Result", true.ToString()},
                    {"Error_message", ""}
         });

            #endregion

        }
    }
}