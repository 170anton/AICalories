using AICalories.ViewModels;

namespace AICalories.Views;

public partial class LoadScreenPage : ContentPage
{
    private LoadScreenVM _viewModel;

    public LoadScreenPage()
    {
        InitializeComponent();
        _viewModel = new LoadScreenVM();
        BindingContext = _viewModel;
    }

    public void LoadAIResponse(string response)
    {
        _viewModel.IsRefreshing = false;
        _viewModel.AIResponse = response;
    }


    protected override bool OnBackButtonPressed()
    {
        //Shell.Current.GoToAsync("//main");
        //Navigation.PopAsync();
        if (_viewModel.AIResponse != null) //should be check on completed result
        {
            return false;
        }
        return true;
    }
}
