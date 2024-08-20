using AICalories.Interfaces;
using AICalories.Models;
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


    public async void ProcessImage(string image)
    {
        if (image != null)
        {
            var response = await _viewModel.ProcessImage(image);
            if (response == null)
            {
                LoadAIResponse("Loading error");
                return;
            }
            LoadAIResponse(response);
        }
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
        if (_viewModel.DishName != null) //should be check on completed result
        {
            Navigation.PopModalAsync();
        }
        return true;
    }
}
