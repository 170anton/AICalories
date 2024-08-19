﻿using AICalories.DI;
using AICalories.Interfaces;
using AICalories.ViewModels;

namespace AICalories.Views;

public partial class ContextPage : ContentPage
{
    private const string SelectedOptionKey = "SelectedOption";

    private ContextVM _viewModel;
    private IImageInfo _imageInfo;


    public ContextPage(IImageInfo ImageInfo)
	{
		InitializeComponent();

        var viewModelLocator = Application.Current.Handler.MauiContext.Services.GetService<ViewModelLocator>();
        if (viewModelLocator == null)
        {
            return;
        }

        _viewModel = viewModelLocator.GetContextViewModel();
        BindingContext = _viewModel;

        _imageInfo = ImageInfo;

        _viewModel.MainImage = _imageInfo.Image;

    }


    private void OnConfirmClicked(System.Object sender, System.EventArgs e)
    {
        var resultPage = new ResultPage(_imageInfo);

        Shell.Current.Navigation.PopModalAsync();
        Shell.Current.Navigation.PushModalAsync(resultPage);

    }

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

    protected override bool OnBackButtonPressed()
    {
        //Shell.Current.GoToAsync("//main");
        Shell.Current.Navigation.PopModalAsync();
        return true;
    }
}
