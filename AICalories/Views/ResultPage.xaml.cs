using AICalories.DI;
using AICalories.Interfaces;
using AICalories.Models;
using AICalories.ViewModels;
using AndroidX.Lifecycle;

namespace AICalories.Views;

public partial class ResultPage : ContentPage
{
	private ResultVM _viewModel;

    public ResultPage()
	{
		InitializeComponent();

        var viewModelLocator = Application.Current.Handler.MauiContext.Services.GetService<ViewModelLocator>();
        if (viewModelLocator == null)
        {
            return;
        }

        _viewModel = viewModelLocator.GetResultViewModel();
        BindingContext = _viewModel;
    }

    //private void OnNewImageClicked(System.Object sender, System.EventArgs e)
    //{

    //    var takeImagePage = IPlatformApplication.Current.Services.GetService<TakeImagePage>();

    //    Shell.Current.Navigation.PopModalAsync();
    //    Shell.Current.Navigation.PushModalAsync(takeImagePage);

    //}

    protected override bool OnBackButtonPressed()
    {
        if (_viewModel.IsLoading == false) //should be check on completed result
        {
            Navigation.PopModalAsync();
        }
        return true;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        _viewModel.LoadAdCommand.Execute(null);
        _viewModel.ShowAdCommand.Execute(null);

        await _viewModel.ProcessImage();

        //// Optionally, you can delay showing the ad to ensure it’s fully loaded
        //Device.StartTimer(TimeSpan.FromSeconds(2), () =>
        //{
        //    _viewModel.ShowAd();
        //    return false; // Run only once
        //});

    }
}
