using System;
using AICalories.ViewModels;
using Android.Gms.Ads;
using Android.Gms.Ads.Interstitial;

namespace AICalories.Services
{
    public interface IAdService
    {
        bool IsAdsEnabled { get; set; }
        //bool IsAdsDismissed { get; set; }
        Task LoadAdAsync();
        Task ShowAd();
        Task SetInterstitialAd(InterstitialAd ad);
    }

    public class AdService : IAdService
    {
        private InterstitialAd _interstitialAd;
        private readonly string _adUnitId = "ca-app-pub-9280044316923474/7763621828";

        public bool IsAdsEnabled { get; set; }
        //public bool IsAdsDismissed { get; set; }

        public async Task LoadAdAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    var adRequest = new AdRequest.Builder().Build();
                    var context = Android.App.Application.Context;


                    InterstitialAd.Load(context, _adUnitId, adRequest,
                        new CustomInterstitialAdLoadCallback(
                            ad =>
                            {
                                SetInterstitialAd(ad);
                                Console.WriteLine("Interstitial Ad loaded successfully.");
                                ShowAd();
                            },
                            loadAdError =>
                            {
                                Console.WriteLine($"Failed to load interstitial ad: {loadAdError.Message}");
                                IsAdsEnabled = false;
                                //if (IsLoading == false)
                                //{
                                //    ShowHistoryGridAfterAds();
                                //}
                            }));
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading ads: {ex.Message}");
                IsAdsEnabled = false;
                //if (IsLoading == false)
                //{
                //    ShowHistoryGridAfterAds();
                //}
            }
        }

        public Task ShowAd()
        {
            var context = Platform.CurrentActivity;

            if (_interstitialAd != null)
            {
                _interstitialAd.Show(context);
            }
            else
            {
                Console.WriteLine("Ad is not ready to be shown yet.");
                IsAdsEnabled = false;
                //if (IsLoading == false)
                //{
                //    ShowHistoryGridAfterAds();
                //}
            }
            return Task.CompletedTask;
        }

        public Task SetInterstitialAd(InterstitialAd ad)
        {
            if (ad != null)
            {
                _interstitialAd = ad;
                _interstitialAd.FullScreenContentCallback = new InternalFullScreenContentCallback(this);
            }
            return Task.CompletedTask;
        }
    }



    public class InternalFullScreenContentCallback : FullScreenContentCallback
    {
        private readonly AdService _adService;

        public InternalFullScreenContentCallback(AdService adService)
        {
            _adService = adService;
        }

        public override void OnAdDismissedFullScreenContent()
        {
            Console.WriteLine("Ad dismissed.");
            _adService.IsAdsEnabled = false;
            //if (_viewModel.IsLoading == false)
            //{
            //    _viewModel.ShowHistoryGridAfterAds();
            //}
        }

        public override void OnAdFailedToShowFullScreenContent(AdError adError)
        {
            Console.WriteLine($"Interstitial ad failed to show: {adError.Message}");
            _adService.IsAdsEnabled = false;
            //if (_viewModel.IsLoading == false)
            //{
            //    _viewModel.ShowHistoryGridAfterAds();
            //}
        }

        public override void OnAdShowedFullScreenContent()
        {
            Console.WriteLine("Interstitial ad showed full screen content.");
            _adService.IsAdsEnabled = true;

            // Set the interstitial ad to null after it is shown to prevent reuse
            _adService.SetInterstitialAd(null); // Clear the ad reference after showing //todo test
        }
    }
}

