using AICalories.ViewModels;

namespace AICalories.Views;

public partial class DonatePage : ContentPage
{
    private DonateVM _viewModel;

    public DonatePage()
	{
		InitializeComponent();
        _viewModel = new DonateVM();
        BindingContext = _viewModel;
    }

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("//history");
        return true;
    }
}
