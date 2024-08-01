namespace AICalories.Views;

public partial class AppSettings : ContentPage
{
	public AppSettings()
	{
		InitializeComponent();
	}

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("//main");
        return true;
    }
}
