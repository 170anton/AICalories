namespace AICalories.Views;

public partial class AIContextPage : ContentPage
{
	public AIContextPage()
	{
		InitializeComponent();
    }

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("//main");
        return true;
    }
}
