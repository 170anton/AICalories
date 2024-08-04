using AICalories.DI;
using AICalories.ViewModels;

namespace AICalories.Views;

public partial class ContextPage : ContentPage
{
    private ContextVM _viewModel;
         
	public ContextPage()
	{
		InitializeComponent();

        var viewModelLocator = Application.Current.Handler.MauiContext.Services.GetService<ViewModelLocator>();
        if (viewModelLocator != null)
        {

            _viewModel = viewModelLocator.GetContextViewModel();
            BindingContext = _viewModel;
        }
        
    }


    private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            var radioButton = sender as RadioButton;
            //var viewModel = BindingContext as OptionsViewModel;
            _viewModel.SelectedOption = radioButton?.Value?.ToString();
        }
    }

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("//main");
        return true;
    }
}
