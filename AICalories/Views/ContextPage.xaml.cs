namespace AICalories.Views;

public partial class ContextPage : ContentPage
{
	public ContextPage()
	{
		InitializeComponent();
    }

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("//main");
        return true;
    }
}
