﻿namespace AICalories.Views;

public partial class LoadScreenPage : ContentPage
{
	public LoadScreenPage()
	{
		InitializeComponent();
    }

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("//main");
        return true;
    }
}
