using AICalories.DI;
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
        if (viewModelLocator != null)
        {
            _viewModel = viewModelLocator.GetContextViewModel();
            BindingContext = _viewModel;

            //if (_viewModel.SelectedOption == null)
            //{
            //    _viewModel.SelectedOption = "Regular";
            //}

            //LoadSelectedOption();
        }

    }


    private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            var radioButton = sender as RadioButton;
            var selectedOption = radioButton?.Value?.ToString();
            // Save the selected option to preferences
            Preferences.Set(SelectedOptionKey, selectedOption);

            _viewModel.SelectedOption = selectedOption;
        }
    }


    private void LoadSelectedOption()
    {
        var savedOption = Preferences.Get(SelectedOptionKey, "Regular");
        if (savedOption != null)
        {
            _viewModel.SelectedOption = savedOption;
        }
    }

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("//main");
        return true;
    }
}
