using AICalories.Interfaces;
using AICalories.ViewModels;

namespace AICalories.Views;

public partial class ResultPage : ContentPage
{
	private ResultVM _viewModel;

    public ResultPage(ResultVM viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}

    protected override bool OnBackButtonPressed()
    {
        //Shell.Current.GoToAsync("//main");
        Shell.Current.Navigation.PopModalAsync();
        return true;
    }
}
