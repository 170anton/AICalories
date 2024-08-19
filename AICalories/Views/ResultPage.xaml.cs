using AICalories.Interfaces;
using AICalories.ViewModels;

namespace AICalories.Views;

public partial class ResultPage : ContentPage
{
	private ResultVM _viewModel;
    private IImageInfo _imageInfo;

    public ResultPage(IImageInfo ImageInfo)
	{
		InitializeComponent();
		_viewModel = new ResultVM();
		BindingContext = _viewModel;
	}

    protected override bool OnBackButtonPressed()
    {
        //Shell.Current.GoToAsync("//main");
        Shell.Current.Navigation.PopModalAsync();
        return true;
    }
}
