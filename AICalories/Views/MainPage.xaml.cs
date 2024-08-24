using System.Data;
using System.Text;
using AICalories;
using AICalories.DI;
using AICalories.Models;
using AICalories.ViewModels;
using Microsoft.Maui.Graphics.Platform;
using Newtonsoft.Json; 

namespace AICalories.Views;

public partial class MainPage : ContentPage
{
    private MainVM _viewModel;

    #region Constructor

    public MainPage()
	{
		InitializeComponent();
        try
        {
            var viewModelLocator = Application.Current.Handler.MauiContext.Services.GetService<ViewModelLocator>();
            if (viewModelLocator != null)
            {
                _viewModel = viewModelLocator.GetMainViewModel();
                BindingContext = _viewModel;
            }

            //_viewModel.LoadLastHistoryItem();
        }
        catch (Exception)
        {
            DisplayAlertConfiguration.ShowUnexpectedError();
        }
    }

    #endregion



    #region View

    private void OnSwipedLeft(object sender, SwipedEventArgs e)
    {
        // Move to the next tab
        //var shell = (AppShell)Application.Current.MainPage;
        //var currentIndex = shell.Items.IndexOf(shell.CurrentItem);
        //if (currentIndex < shell.Items.Count - 1)
        //{
        //    shell.CurrentItem = shell.Items[currentIndex + 1];
        //}
        Shell.Current.GoToAsync("//history");
    }

    private void OnSwipedRight(object sender, SwipedEventArgs e)
    {
        // Move to the previous tab
        //var shell = (AppShell)Application.Current.MainPage;
        //var currentIndex = shell.Items.IndexOf(shell.CurrentItem);
        //if (currentIndex > 0)
        //{
        //    shell.CurrentItem = shell.Items[currentIndex - 1];
        //}

        Shell.Current.GoToAsync("//settings");
    }



    protected override void OnDisappearing()
    {
        base.OnDisappearing();

    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.OnPageAppearingAsync();
    }
    #endregion

}