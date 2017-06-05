using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Gms.Common.Apis;
using Android.Gms.Fitness;
using Android.Gms.Fitness.Data;
using Android.Gms.Fitness.Request;
using Android.Gms.Fitness.Result;
using Google.XamarinSamples.XamFit;
using Java.IO;
using Java.Util.Concurrent;
using MobileCenterDemoApp.Droid.Dependencies;
using MobileCenterDemoApp.Interfaces;
using Xamarin.Forms;
using System.Threading;

[assembly: Dependency(typeof(GoogleFitImplementation))]
namespace MobileCenterDemoApp.Droid.Dependencies
{
    public class GoogleFitImplementation : IFitnessTracker
    {
        public string ApiName => "Google fit";

        public event Action<string> OnError;
        public event Action OnConnect;

        private static GoogleApiClient Client => MainActivity.Activity.MClient;

        public bool IsConnected => Client != null && Client.IsConnected;

        public async void StepsByPeriod(DateTime start, DateTime end, Action<IEnumerable<int>> act)
        {
            start = TimeZoneInfo.ConvertTimeToUtc(start);
            end = TimeZoneInfo.ConvertTimeToUtc(end);

            using (DataReadRequest stepRequest =CreateRequest(DataType.TypeStepCountDelta, DataType.AggregateStepCountDelta, start, end))
            using (IResult stepResult = await ReadData(stepRequest))
                act?.Invoke(GetIntFromResult(stepResult));
        }

        public async void DistanceByPeriod(DateTime start, DateTime end, Action<IEnumerable<double>> act)
        {
            start = TimeZoneInfo.ConvertTimeToUtc(start);
            end = TimeZoneInfo.ConvertTimeToUtc(end);

            using (DataReadRequest distanceRequest = CreateRequest(DataType.TypeDistanceDelta, DataType.AggregateDistanceDelta, start, end))
            using (IResult distanceResult = await ReadData(distanceRequest))
                act?.Invoke(GetFloatFromResult(distanceResult).Select(x => (double)(x)));
        }

        public async void CaloriesByPeriod(DateTime start, DateTime end, Action<IEnumerable<double>> act)
        {
            start = TimeZoneInfo.ConvertTimeToUtc(start);
            end = TimeZoneInfo.ConvertTimeToUtc(end);

            using (DataReadRequest caloriesRequest = new DataReadRequest.Builder()
                .Aggregate(DataType.TypeCaloriesExpended, DataType.AggregateCaloriesExpended)
                .BucketByTime(1, TimeUnit.Days)
                .SetTimeRange(TimeUtility.DatetimeInMillis(start), TimeUtility.DatetimeInMillis(end), TimeUnit.Milliseconds)
                .Build())
            using (DataReadResult caloriesResult = (DataReadResult)await ReadData(caloriesRequest))
                act?.Invoke(GetFloatFromResult(caloriesResult).Select(x => Math.Round(x)));
        }

        public async void ActiveTimeByPeriod(DateTime start, DateTime end, Action<IEnumerable<TimeSpan>> act)
        {
            start = TimeZoneInfo.ConvertTimeToUtc(start);
            end = TimeZoneInfo.ConvertTimeToUtc(end);

            using (DataReadRequest time = new DataReadRequest.Builder()
                .Aggregate(DataType.TypeActivitySegment, DataType.AggregateActivitySummary)
                .BucketByTime(1, TimeUnit.Days)
                .SetTimeRange(TimeUtility.DatetimeInMillis(start), TimeUtility.DatetimeInMillis(end), TimeUnit.Milliseconds)
                .Build())
            using (DataReadResult caloriesResult = (DataReadResult)await ReadData(time))
                act?.Invoke(GetIntFromResult(caloriesResult, new string[] { "duration" }).Select(x => TimeSpan.FromMinutes(x)));
        }

        private async Task<IResult> ReadData(DataReadRequest request) 
            => await FitnessClass.HistoryApi.ReadData(Client, request);

        public async Task Connect()
        {
            if(Client == null)
                throw new NotActiveException();

            if(Client.IsConnecting || Client.IsConnected)
                return;

            try
            {
                Client.Connect();

                await Task.Run(() =>
                {
                    // Await connectiong
                    while (Client.IsConnecting)
                    {
                        Task.Delay(50);
                    }
                });

                if (Client.IsConnected)
                    OnConnect?.Invoke();
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
            }
        }

        public void Disconnect()
        {
            if (Client == null)
                throw new NotActiveException();

            if (!Client.IsConnected)
                return;
            try
            {
                Client.Disconnect();
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
            }
        }

        public void Dispose() => Client?.Dispose();

        #region Implements the Google Fit data access        

        private DataReadRequest CreateRequest(DataType input, DataType output, DateTime start, DateTime end)
            => new DataReadRequest.Builder()
                .Aggregate(input, output)
                .BucketByTime(1, TimeUnit.Days)
                .SetTimeRange(TimeUtility.DatetimeInMillis(start), TimeUtility.DatetimeInMillis(end), TimeUnit.Milliseconds)                
                .Build();

        private IEnumerable<int> GetIntFromResult(IResult result, string[] skip = null)
        {
            if (result == null)
                return null;

            IList<Bucket> buckets = ((DataReadResult)result).Buckets;
            if (!buckets.Any())
                return null;
            return buckets.Select(x => ExtractInt(x, skip)).ToArray();
        }

        private IEnumerable<float> GetFloatFromResult(IResult result)
        {
            if (result == null)
                return null;

            IList<Bucket> buckets = ((DataReadResult)result).Buckets;
            if (!buckets.Any())
                return null;

            return buckets.Select(ExtractFloat).ToArray();
        }

        private int ExtractInt(Bucket bucket, string[] skipTypes = null)
        {
            var en = from ds in bucket.DataSets
                     from p in ds.DataPoints
                     from f in p.DataType.Fields
                     select new { typeName = f.Name, value = p.GetValue(f).AsInt() };

            if(skipTypes == null)
                return en.Sum(x => x.value);

            return en.Where(x => !skipTypes.Contains(x.typeName)).Sum(x => x.value);
        }


        private float ExtractFloat(Bucket bucket)
            => (from ds in bucket.DataSets
                from p in ds.DataPoints
                from f in p.DataType.Fields
                select p.GetValue(f).AsFloat()).Sum();

        

        #endregion
    }
}