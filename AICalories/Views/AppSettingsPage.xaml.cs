using AICalories.ViewModels;

namespace AICalories.Views;

public partial class AppSettingsPage : ContentPage
{
    private AppSettingsVM _viewModel;

    public AppSettingsPage()
	{
		InitializeComponent();
        _viewModel = new AppSettingsVM();
        BindingContext = _viewModel;
	}

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("//history");
        return true;
    }
}
