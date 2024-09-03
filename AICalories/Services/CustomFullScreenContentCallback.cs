using System;
using AICalories.ViewModels;
using AICalories.Views;
using Android.Gms.Ads;
using Android.Views;
using AndroidX.Lifecycle;

namespace AICalories.Services
{
    public class CustomFullScreenContentCallback : FullScreenContentCallback
    {
        private readonly ResultVM _viewModel;

        public CustomFullScreenContentCallback(ResultVM viewModel)
        {
            _viewModel = viewModel;
        }

        public override void OnAdDismissedFullScreenContent()
        {
            Console.WriteLine("Interstitial ad dismissed.");
            _viewModel.IsAdsVisible = false;
            if (_viewModel.IsLoading == false)
            {
                _viewModel.IsHistoryGridVisible = true;
            }
        }

        public override void OnAdFailedToShowFullScreenContent(AdError adError)
        {
            Console.WriteLine($"Interstitial ad failed to show: {adError.Message}");
        }

        public override void OnAdShowedFullScreenContent()
        {
            Console.WriteLine("Interstitial ad showed full screen content.");
            _viewModel.IsAdsVisible = true;

            // Set the interstitial ad to null after it is shown to prevent reuse
            _viewModel.SetInterstitialAd(null); // Clear the ad reference after showing //todo test
        } 
    }
}

