using System.Windows.Input;
using AICalories.CustomControls;
using AICalories.DI;
using AICalories.Interfaces;
using AICalories.Models;
using AICalories.ViewModels;

namespace AICalories.Views;

public partial class ContextPage : ContentPage
{
    private const string SelectedOptionKey = "SelectedOption";
    private readonly IKeyboardHelper _keyboardHelper;

    private ContextVM _viewModel;

    public ContextPage(IKeyboardHelper keyboardHelper)
	{
		InitializeComponent();

        var viewModelLocator = Microsoft.Maui.Controls.Application.Current.Handler.MauiContext.Services.GetService<ViewModelLocator>();
        if (viewModelLocator == null)
        {
            return;
        }

        _viewModel = viewModelLocator.GetContextViewModel();
        BindingContext = _viewModel;
        _keyboardHelper = keyboardHelper;


        if (BindingContext is ContextVM viewModel)
        {
            //viewModel.ContextGridAdded += OnContextGridAdded;
        }
        //_viewModel.UpdateContextList(contextLayout);



        var tapToHideKeyboard = new TapGestureRecognizer();
        tapToHideKeyboard.Tapped += OnHideKeyboardTapped;
        mainLayout.GestureRecognizers.Add(tapToHideKeyboard);
    }

    //private async void AddNewContextClicked(System.Object sender, System.EventArgs e) //todo move to vm
    //{
    //    var newContextItem = new ContextItem() { Text = "First", IsSelected = true };
    //    await App.ContextItemRepository.InsertContextItemAsync(newContextItem);

    //    newContextItem = await App.ContextItemRepository.GetLastAddedContextItemAsync();
    //    var newContextGrid = new ContextGrid(newContextItem.Id, newContextItem.Text, newContextItem.IsSelected,
    //        App.ContextItemRepository, contextLayout);

    //    contextLayout.Children.Add(newContextGrid);
    //    //await _viewModel.UpdateContextList(contextLayout);
    //}

    //private void OnContextGridAdded(ContextGrid contextGrid) //todo do not delete
    //{
    //    // Add the new ContextGrid to the VerticalStackLayout
    //    contextLayout.Children.Add(contextGrid);
    //}


    //private async void OnConfirmClicked(System.Object sender, System.EventArgs e)
    //{
    //    //var resultPage = new ResultPage();
    //    _viewModel.SetAdditionalInfo();

    //    if (!InternetConnection.CheckInternetConnection())
    //    {
    //        DisplayAlertConfiguration.ShowError("No internet connection");
    //        return;
    //    }
    //    var resultPage = IPlatformApplication.Current.Services.GetService<ResultPage>();
    //    Shell.Current.Navigation.PopModalAsync();
    //    await Shell.Current.Navigation.PushModalAsync(resultPage);
    //    resultPage?.ProcessImage(_viewModel.MainImage);

    //}


    //private void OnNewImageClicked(System.Object sender, System.EventArgs e)
    //{

    //    var takeImagePage = IPlatformApplication.Current.Services.GetService<TakeImagePage>();

    //    Shell.Current.Navigation.PopModalAsync();
    //    Shell.Current.Navigation.PushModalAsync(takeImagePage);

    //}
    private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            var radioButton = sender as RadioButton;
            var selectedOption = radioButton?.Value?.ToString();
            
            Preferences.Set(SelectedOptionKey, selectedOption);
            
            _viewModel.SelectedOption = selectedOption;
        }
    }

    private void OnHideKeyboardTapped(object sender, EventArgs e)
    {
        //var keyboardHelper = DependencyService.Get<IKeyboardHelper>();
        _keyboardHelper?.HideKeyboard();
        userInfoEditor.Unfocus();
    }

    protected override bool OnBackButtonPressed()
    {
        //Shell.Current.GoToAsync("//main");
        var takeImagePage = IPlatformApplication.Current.Services.GetService<TakeImagePage>();

        Shell.Current.Navigation.PopModalAsync();
        Shell.Current.Navigation.PushModalAsync(takeImagePage);
        return true;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (BindingContext is ContextVM viewModel)
        {
            //viewModel.ContextGridAdded -= OnContextGridAdded;
        }
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        //await _viewModel.UpdateContextList(contextLayout);
        

    }
}
