namespace AICalories.Views;

public partial class HomePage : TabbedPage
{
	public HomePage()
	{
		InitializeComponent();
	}

    private void OnMenuButtonClicked(object sender, EventArgs e)
    {
        var flyoutPage = (Application.Current.MainPage as LeftSidePanelPage);
        if (flyoutPage != null)
        {
            flyoutPage.IsPresented = true;
        }
    }
}
