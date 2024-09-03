using System;
using AICalories.ViewModels;
using AICalories.Views;
using Android.Gms.Ads;
using Android.Gms.Ads.Hack;
using Android.Gms.Ads.Interstitial;
using Android.Runtime;
using AndroidX.Lifecycle;

namespace AICalories.Services
{
    public class CustomInterstitialAdLoadCallback : InterstitialCallback
    {
        //private readonly ResultVM _viewModel;

        //public CustomInterstitialAdLoadCallback(ResultVM viewModel)
        //{
        //    _viewModel = viewModel;
        //}



        //public override void OnAdLoaded(Java.Lang.Object p0)
        //{
        //    if (p0 is Android.Gms.Ads.Interstitial.InterstitialAd ad)
        //    {
        //        Console.WriteLine("Interstitial ad loaded successfully.");
        //        _viewModel.SetInterstitialAd(ad);
        //    }
        //}

        //public override void OnAdFailedToLoad(LoadAdError adError)
        //{
        //    Console.WriteLine($"Failed to load interstitial ad: {adError.Message}");
        //}

        private readonly Action<InterstitialAd> _onAdLoaded;
        private readonly Action<LoadAdError> _onAdFailedToLoad;

        public CustomInterstitialAdLoadCallback(Action<InterstitialAd> onAdLoaded, Action<LoadAdError> onAdFailedToLoad)
        {
            _onAdLoaded = onAdLoaded;
            _onAdFailedToLoad = onAdFailedToLoad;
        }

        public override void OnAdLoaded(InterstitialAd ad)
        {
            base.OnAdLoaded(ad);
            _onAdLoaded?.Invoke(ad);
        }

        public override void OnAdFailedToLoad(LoadAdError adError)
        {
            base.OnAdFailedToLoad(adError);
            _onAdFailedToLoad?.Invoke(adError);
        }

        //public static void LoadInterstitial(Android.Content.Context context, string Adunit, Android.Gms.Ads.AdRequest adRequest, Android.Gms.Ads.AdLoadCallback Callback)
        //{
        //    IntPtr AdRequestClass = JNIEnv.GetObjectClass(adRequest.Handle);
        //    IntPtr zzdsmethodID = JNIEnv.GetMethodID(AdRequestClass, "zza", "()Lcom/google/android/gms/internal/ads/zzbdq;");
        //    Java.Lang.Object zzzk = GetObject<Java.Lang.Object>(JNIEnv.CallObjectMethod(adRequest.Handle, zzdsmethodID), JniHandleOwnership.TransferLocalRef);

        //    IntPtr zzakjtype = JNIEnv.FindClass("com/google/android/gms/internal/ads/zzbof");
        //    IntPtr zzakjConstructor = JNIEnv.GetMethodID(zzakjtype, "<init>", "(Landroid/content/Context;Ljava/lang/String;)V");
        //    IntPtr zzakjInstance = JNIEnv.NewObject(zzakjtype, zzakjConstructor, new JValue[] { new JValue(context), new JValue(new Java.Lang.String(Adunit)) });

        //    IntPtr LoadMethodId = JNIEnv.GetMethodID(zzakjtype, "zza", "(Lcom/google/android/gms/internal/ads/zzbdq;Lcom/google/android/gms/ads/AdLoadCallback;)V");
        //    JNIEnv.CallVoidMethod(zzakjInstance, LoadMethodId, new JValue[] { new JValue(zzzk), new JValue(Callback) });
        //}
    }
}

