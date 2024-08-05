namespace AICalories.Views;

public partial class SupportPage : ContentPage
{
	public SupportPage()
	{
		InitializeComponent();
    }

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("//history");
        return true;
    }
}
