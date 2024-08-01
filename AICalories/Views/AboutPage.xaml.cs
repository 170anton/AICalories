namespace AICalories.Views;

public partial class AboutPage : ContentPage
{
    public string AppVersionLabel { get; set; }

    public AboutPage()
    {
        AppVersionLabel = $"Version: {AppInfo.VersionString}";
        InitializeComponent();
        BindingContext = this;
    }

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("//main");
        return true;
    }
}
