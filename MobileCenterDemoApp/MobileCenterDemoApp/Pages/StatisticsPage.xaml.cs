using System;
using System.Collections.Generic;
using Microsoft.Azure.Mobile.Analytics;
using MobileCenterDemoApp.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileCenterDemoApp.Pages
{
	public partial class StatisticsPage : ContentPage
	{
		public StatisticsPage ()
		{
			InitializeComponent ();
		}

	    private bool _firstAppear = true;

	    protected override void OnAppearing()
	    {
	        if(!_firstAppear)
                return;

	        _firstAppear = false;

            Analytics.TrackEvent("View statistics button clicked", new Dictionary<string, string>
	        {
	            {"Page", "Main" },
	            {"Category", "Clicks" }
	        });

	        string errorMessage = "";
	        bool success;
	        try
	        {
	            DataStore.ReadStatisticsInformation();
	            success = true;
	        }
	        catch (Exception e)
	        {
	            errorMessage = e.Message;
	            success = false;
	        }

	        Analytics.TrackEvent("Trying to retrieve data from HealthKit/Google Fit API.",
	            new Dictionary<string, string>
	            {
	                {"Page", "Main"},
	                {"Category", "Request"},
	                {
	                    "API",
	                    DataStore.FitnessTracker?.ApiName ?? (Device.RuntimePlatform == Device.Android ? "Google fit" : "HealthKit")
	                },
	                {"Result", success.ToString()},
	                {"Error_message", errorMessage}
	            });

            base.OnAppearing();
	    }
	}
}
