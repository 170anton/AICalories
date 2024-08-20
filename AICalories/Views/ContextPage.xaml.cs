using AICalories.DI;
using AICalories.Interfaces;
using AICalories.Models;
using AICalories.ViewModels;

namespace AICalories.Views;

public partial class ContextPage : ContentPage
{
    private const string SelectedOptionKey = "SelectedOption";

    private ContextVM _viewModel;


    public ContextPage()
	{
		InitializeComponent();

        var viewModelLocator = Application.Current.Handler.MauiContext.Services.GetService<ViewModelLocator>();
        if (viewModelLocator == null)
        {
            return;
        }

        _viewModel = viewModelLocator.GetContextViewModel();
        BindingContext = _viewModel;

    }


    private void OnConfirmClicked(System.Object sender, System.EventArgs e)
    {
        //var resultPage = new ResultPage();
        _viewModel.SetAdditionalInfo();

        if (!InternetConnection.CheckInternetConnection())
        {
            DisplayAlertConfiguration.ShowError("No internet connection");
            return;
        }


        var resultPage = IPlatformApplication.Current.Services.GetService<ResultPage>();
        Shell.Current.Navigation.PopModalAsync();
        Shell.Current.Navigation.PushModalAsync(resultPage);
        resultPage.ProcessImage(_viewModel.MainImage);

    }

    private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            var radioButton = sender as RadioButton;
            var selectedOption = radioButton?.Value?.ToString();
            
            Preferences.Set(SelectedOptionKey, selectedOption);

            _viewModel.SelectedOption = selectedOption;
        }
    }

    protected override bool OnBackButtonPressed()
    {
        //Shell.Current.GoToAsync("//main");
        Shell.Current.Navigation.PopModalAsync();
        return true;
    }
}
