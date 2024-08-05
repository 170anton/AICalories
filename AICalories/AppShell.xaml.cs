using AICalories.DI;
using AICalories.Views;

namespace AICalories;

public partial class AppShell : Shell
{
    private readonly ViewModelLocator _viewModelLocator;
    private bool isNavigating = false;
    public string AppVersionLabel { get; set; }

    public AppShell(ViewModelLocator viewModelLocator)
    {
        AppVersionLabel = $"Version: Alpha {AppInfo.VersionString}";
        InitializeComponent();
        _viewModelLocator = viewModelLocator;
        BindingContext = this;

        // Ensure your pages get their view models
        Routing.RegisterRoute("main", typeof(PhotoSelectionPage));
        Routing.RegisterRoute("context", typeof(ContextPage));
        

        //Routing.RegisterRoute(nameof(ContextPage), typeof(ContextPage));
        //Routing.RegisterRoute(nameof(PhotoSelectionPage), typeof(PhotoSelectionPage));

        this.Navigating += OnNavigating;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        //Shell.Current.GoToAsync("//context");
        //Shell.Current.GoToAsync("//main");
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
