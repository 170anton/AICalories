using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AICalories.Models;
using AICalories.ViewModels;

namespace AICalories.Views;

public partial class HistoryPage : ContentPage, INotifyPropertyChanged
{
    private HistoryVM _viewModel;


    public HistoryPage()
    {
        InitializeComponent();
        _viewModel = new HistoryVM();
        BindingContext = _viewModel;
    }

    private void ShowOverlay()
    {
        //blurBackground.IsVisible = true;
        //overlayFrame.IsVisible = true;
    }

    private void HideOverlay()
    {
        //blurBackground.IsVisible = false;
        //overlayFrame.IsVisible = false;
        
    }

    private void OnAddToHeaderButton_Clicked(System.Object sender, System.EventArgs e)
    {
        ShowOverlay();
    }

    private void OnCloseButtonClicked(object sender, EventArgs e)
    {
        HideOverlay();
    }

    public async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            var item = e.CurrentSelection;

            // Ensure there is a selected item
            if (item == null || item.Count == 0)
            {
                return;
            }

            if (item.FirstOrDefault() is MealItem selectedItem)
            {
                bool delete = await DisplayAlert("Delete", "Are you sure to delete it?", "Yes", "No");
                if (delete)
                {
                    _viewModel.DeleteItemCommand.Execute(selectedItem);
                }
            }
            // Deselect the item
            ((CollectionView)sender).SelectedItem = null;
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "Sad");
        }
        
    }

    private void OnSwipedRight(object sender, SwipedEventArgs e)
    {
        Shell.Current.GoToAsync("//main");
    }

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("//main");
        return true;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.CheckForUpdate();
    }
}
