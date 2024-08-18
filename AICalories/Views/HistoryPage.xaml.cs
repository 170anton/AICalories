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
        //Task.Run(() => §.LoadData()); //do not await
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

            if (item.FirstOrDefault() is HistoryItem selectedItem)
            {
                bool delete = await DisplayAlert("Delete Item", "Do you want to delete this item?", "Yes", "No");
                if (delete)
                {
                    if (_viewModel != null)
                    {
                        _viewModel.DeleteItemCommand.Execute(selectedItem);
                    }
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
