using AICalories.DI;
using AICalories.ViewModels;
using AICalories.Views;

namespace AICalories;


public partial class App : Application
{
    static DatabaseHelper database;

    public static DatabaseHelper Database
    {
        get
        {
            if (database == null)
            {
                database = new DatabaseHelper(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AICaloriesDatabase.db3"));
            }
            return database;
        }
    }

    public App(IServiceProvider serviceProvider)
	{
		InitializeComponent();
        MainPage = serviceProvider.GetRequiredService<AppShell>();

        // Ensure ContextPage is initialized
        InitializeContextPage(serviceProvider);
    }

    private void InitializeContextPage(IServiceProvider serviceProvider)
    {
        var contextVM = serviceProvider.GetRequiredService<ContextVM>();
    }
}
