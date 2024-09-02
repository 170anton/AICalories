using System;
using AICalories.ViewModels;
using AICalories.Views;
using Android.Gms.Ads;
using Android.Gms.Ads.Interstitial;
using AndroidX.Lifecycle;

namespace AICalories.Services
{
    public class CustomInterstitialAdLoadCallback : InterstitialAdLoadCallback
    {
        private readonly ResultVM _viewModel;

        public CustomInterstitialAdLoadCallback(ResultVM viewModel)
        {
            _viewModel = viewModel;
        }



        //public override void OnAdLoaded(Java.Lang.Object p0)
        //{
        //    if (p0 is Android.Gms.Ads.Interstitial.InterstitialAd ad)
        //    {
        //        Console.WriteLine("Interstitial ad loaded successfully.");
        //        _viewModel.SetInterstitialAd(ad);
        //    }
        //}

        //public override void OnAdLoaded(Object p0)
        //{
        //    if (p0 is InterstitialAd ad)
        //    {
        //        Console.WriteLine("Interstitial ad loaded successfully.");
        //        result.SetInterstitialAd(ad);
        //    }
        //}

        public override void OnAdFailedToLoad(LoadAdError adError)
        {
            Console.WriteLine($"Failed to load interstitial ad: {adError.Message}");
        }

    }
}

