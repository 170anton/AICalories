using AICalories.DI;
using AICalories.ViewModels;

namespace AICalories.Views;

public partial class AppSettingsPage : ContentPage
{
    private AppSettingsVM _viewModel;

    public AppSettingsPage()
	{
		InitializeComponent();
        var viewModelLocator = Application.Current.Handler.MauiContext.Services.GetService<ViewModelLocator>();
        if (viewModelLocator != null)
        {
            _viewModel = viewModelLocator.GetAppSettingsViewModel();
            BindingContext = _viewModel;

            //if (_viewModel.SelectedOption == null)
            //{
            //    _viewModel.SelectedOption = "Regular";
            //}

            //LoadSelectedOption();
        }
	}

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("//history");
        return true;
    }
}
