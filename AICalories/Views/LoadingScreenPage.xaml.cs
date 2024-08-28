using AICalories.Models;
using AICalories.ViewModels;

namespace AICalories.Views;

public partial class LoadingScreenPage : ContentPage
{
    private LoadingScreenVM _viewModel;

    public LoadingScreenPage()
    {
        InitializeComponent();
        _viewModel = new LoadingScreenVM();
        BindingContext = _viewModel;
    }

    public void LoadAIResponse(ResponseData response)
    {
        _viewModel.IsRefreshing = false;
        _viewModel.DishName = response.MealName;
        _viewModel.Calories = response.Calories.ToString();
        _viewModel.TotalResultJSON = response.TotalResultJSON;
    }

    public void LoadAIResponse(string response)
    {
        _viewModel.IsRefreshing = false;
        _viewModel.DishName = response;
    }

    protected override bool OnBackButtonPressed()
    {
        //Shell.Current.GoToAsync("//main");
        //Navigation.PopAsync();
        if (_viewModel.DishName != null) //should be check on completed result
        {
            return false;
        }
        return true;
    }
}
