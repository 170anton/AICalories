using AICalories.DI;
using AICalories.Interfaces;
using AICalories.ViewModels;

namespace AICalories.Views;

public partial class AppSettingsPage : ContentPage
{
    private AppSettingsVM _viewModel;

    public AppSettingsPage()
	{
		InitializeComponent();
        var viewModelLocator = Application.Current.Handler.MauiContext.Services.GetService<ViewModelLocator>();
        if (viewModelLocator == null)
        {
            //throw new NullReferenceException("Dependency has not found");
            return;
        }
        _viewModel = viewModelLocator.GetAppSettingsViewModel();
        BindingContext = _viewModel;

        //editor.SizeChanged += OnEditorSizeChanged;

    }


    private void OnEditorSizeChanged(object sender, EventArgs e)
    {
        var editor = sender as Editor;
        if (editor != null)
        {
            var frame = editor.Parent as Frame;
            if (frame != null)
            {
                // Update frame's height based on editor's height
                frame.HeightRequest = editor.Height;
            }
        }
    }

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("//main");
        return true;
    }

    protected async override void OnAppearing()
    {
        //_viewModel.IsLoading = true;
        base.OnAppearing();
        //await Task.Delay(1000);
        //_viewModel.IsLoading = false;
    }
}
