using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using HealthKit;
using MobileCenterDemoApp.iOS.Dependencies;
using MobileCenterDemoApp.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(HealthKitImplementation))]
namespace MobileCenterDemoApp.iOS.Dependencies
{
    public class HealthKitImplementation : IFitnessTracker
    {
        private readonly HKHealthStore _healthStore;

        public HealthKitImplementation()
        {
            _healthStore = new HKHealthStore();
        }

        private readonly HKQuantityType StepType = HKQuantityType.Create(HKQuantityTypeIdentifier.StepCount);
        private readonly HKQuantityType BasalCaloriesType = HKQuantityType.Create(HKQuantityTypeIdentifier.BasalEnergyBurned);
        private readonly HKQuantityType ActiveCaloriesType = HKQuantityType.Create(HKQuantityTypeIdentifier.ActiveEnergyBurned);
        private readonly HKQuantityType DistanceType = HKQuantityType.Create(HKQuantityTypeIdentifier.DistanceWalkingRunning);
        private readonly HKQuantityType ActiveTimeType = HKQuantityType.Create(HKQuantityTypeIdentifier.AppleExerciseTime);

        #region IFitnessTracker

        public event Action<string> OnError;

        public event Action OnConnect;

        public string ApiName => "HealthKit";

        public bool IsConnected { get; private set; }

        public void StepsByPeriod(DateTime start, DateTime end, Action<IEnumerable<int>> act)
        {
            GetDataFromQuery(start, end, StepType, HKUnit.Count, (obj) => act?.Invoke(obj.Select(Convert.ToInt32)));
        }

        public void DistanceByPeriod(DateTime start, DateTime end, Action<IEnumerable<double>> act)
        {
            GetDataFromQuery(start, end, DistanceType, HKUnit.CreateMeterUnit(HKMetricPrefix.None), act);
        }

        public void CaloriesByPeriod(DateTime start, DateTime end, Action<IEnumerable<double>> act)
        {
            IEnumerable<double> basal = null;
            IEnumerable<double> active = null;

            Action localAct = () =>
            {
                if (basal != null && active != null)
                {
                    double[] basalArray = basal.ToArray();
                    double[] activeArray = active.ToArray();

					int resultArrayLenght = basalArray.Length < activeArray.Length ? activeArray.Length : basalArray.Length;

					double[] result = new double[resultArrayLenght];

					for (int i = 0; i < resultArrayLenght; i++)
						result[i] = (basalArray.Length > i ? basalArray[i] : 0) + (basalArray.Length > i ? activeArray[i] : 0);

                    act?.Invoke(result);
				}                    
            };

            GetDataFromQuery(start, end, BasalCaloriesType, HKUnit.Kilocalorie, (obj) =>
            {
                basal = obj;
                localAct();
            });
			GetDataFromQuery(start, end, ActiveCaloriesType, HKUnit.Kilocalorie, (obj) =>
			{
                active = obj;
				localAct();
			});

        }

        public void ActiveTimeByPeriod(DateTime start, DateTime end, Action<IEnumerable<TimeSpan>> act)
        {
            GetDataFromQuery(start, end, ActiveTimeType, HKUnit.Minute, (obj) => act(obj.Select(TimeSpan.FromMinutes).AsEnumerable()));
        }

        public async Task Connect()
        {

            NSSet readTypes = NSSet.MakeNSObjectSet(new HKObjectType[]
            {
                StepType,
                DistanceType,
                BasalCaloriesType,
                ActiveCaloriesType,
                ActiveTimeType
            });

            try
            {
                if (HKHealthStore.IsHealthDataAvailable)
                {
                    Tuple<bool, NSError> success = await _healthStore.RequestAuthorizationToShareAsync(new NSSet(), readTypes);

                    IsConnected = success.Item1;

                    if (IsConnected)
                    {
                        OnConnect?.Invoke();
                    }
                    else
                    {
                        OnError?.Invoke(success.Item2.Description);
                    }
                }
                else
                {
                    OnError?.Invoke("Is_Health_Data_not_Available".ToUpper());
                }
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
            }
        }

        public void Disconnect()
        {

        }

        #endregion IFitnessTracker

        #region IDisposable

        public void Dispose()
        {
            _healthStore.Dispose();
        }

        #endregion IDisposable

        private void GetDataFromQuery(DateTime start, DateTime end, HKQuantityType quentityType, HKUnit unit, Action<IEnumerable<double>> act)
        {
            NSCalendar calendar = NSCalendar.CurrentCalendar;
            NSDateComponents interval = new NSDateComponents { Day = 1 };
            NSDate startDate = start.ToNsDate();
            NSDate anchorDate = end.ToNsDate();

            HKStatisticsCollectionQuery query = new HKStatisticsCollectionQuery(
                quentityType,
                null,
                HKStatisticsOptions.CumulativeSum,
                anchorDate,
                interval
            )
            {
                InitialResultsHandler = (localQuery, result, error) =>
                {
                    if (error != null)
                    {
                        OnError?.Invoke(error.Description);
                        return;
                    }
                    int daysCount = (end - start).Days + 1;
                    double[] st = new double[daysCount];

					result.EnumerateStatistics(startDate, anchorDate, (statistics, stop) =>
					{
						HKQuantity quantity = statistics?.SumQuantity();

						int index = (statistics.StartDate.ToDateTime() - start).Days;

						if (index < 0 || index > st.Length)
						{
							return;
						}

						double value = quantity?.GetDoubleValue(unit) ?? 0;

						st[index] = value;
					});

					act(st.AsEnumerable());
                }
            };

            try
            {
                _healthStore.ExecuteQuery(query);
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
            }
        }
    }
}