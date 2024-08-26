using AICalories.DI;
using AICalories.Interfaces;
using AICalories.Models;
using AICalories.ViewModels;

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

        await _viewModel.ProcessImage();


    }
}
