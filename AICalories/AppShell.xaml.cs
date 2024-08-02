namespace AICalories;

public partial class AppShell : Shell
{
    private bool isNavigating = false;
    public string AppVersionLabel { get; set; }

    public AppShell()
    {
        AppVersionLabel = $"Version: Alpha {AppInfo.VersionString}";
        InitializeComponent();
        BindingContext = this;
        this.Navigating += OnNavigating;
    }

    private void OnNavigating(object sender, ShellNavigatingEventArgs e)
    {
        if (isNavigating) return;

        try
        {
            var a = e.Target.Location.OriginalString;
            var b = Shell.Current.CurrentState.Location.OriginalString;
            isNavigating = true;
            // Check if the user is navigating to the first tab's route
            if (e.Target.Location.OriginalString == "//context" && Shell.Current.CurrentState.Location.OriginalString == "//history")
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
