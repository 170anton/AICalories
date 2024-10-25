using AICalories.DI;
using AICalories.Models;
using AICalories.ViewModels;

namespace AICalories.Views;

public partial class MealInfoPage : ContentPage
{
    private MealInfoVM _viewModel;
    private MealItem _mealItem;

    public MealInfoPage(MealItem mealItem)
	{
		InitializeComponent();
        try
        {
            var viewModelLocator = Application.Current.Handler.MauiContext.Services.GetService<ViewModelLocator>();
            if (viewModelLocator != null)
            {
                _viewModel = viewModelLocator.GetMealInfoViewModel();
                BindingContext = _viewModel;
            }


            _mealItem = mealItem;
        }
        catch (Exception)
        {
            DisplayAlertConfiguration.ShowUnexpectedError();
        }
    }


    protected override bool OnBackButtonPressed()
    {
        Shell.Current.Navigation.PopModalAsync();
        
        return true;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.LoadMealInfo(_mealItem);
        

    }
}
