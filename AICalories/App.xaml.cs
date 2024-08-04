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
    }
}
