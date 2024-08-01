namespace AICalories;

public partial class AppShell : Shell
{
    private bool isNavigating = false;

    public AppShell()
	{
		InitializeComponent();
        this.Navigating += OnNavigating;
    }

    private void OnNavigating(object sender, ShellNavigatingEventArgs e)
    {
        if (isNavigating) return;

        try
        {
            isNavigating = true;
            // Check if the user is navigating to the first tab's route
            if (e.Target.Location.OriginalString == "//main" && Shell.Current.CurrentState.Location.OriginalString != "//main")
            {
                // Ensure they are on the MainPage
                Shell.Current.GoToAsync("//main");
            }
        }
        finally
        {
            isNavigating = false;
        }
    }
}
