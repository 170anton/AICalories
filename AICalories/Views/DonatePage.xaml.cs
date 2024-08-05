namespace AICalories.Views;

public partial class DonatePage : ContentPage
{
	public DonatePage()
	{
		InitializeComponent();
    }

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("//history");
        return true;
    }
}
