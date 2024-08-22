using AICalories.DI;
using AICalories.Models;
using AICalories.Repositories;
using AICalories.ViewModels;
using AICalories.Views;

namespace AICalories;


public partial class App : Application
{
    private static HistoryDatabase _historyDatabase;
    private static ContextDatabase<ContextItem> _contextDatabase;

    public static IContextItemRepository ContextItemRepository { get; private set; }


    public static HistoryDatabase HistoryDatabase
    {
        get
        {
            if (_historyDatabase == null)
            {
                _historyDatabase = new HistoryDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AICaloriesDatabase.db3")); //todo change to AICaloriesHistoryDatabase
            }
            return _historyDatabase;
        }
    }

    public static ContextDatabase<ContextItem> ContextDatabase
    {
        get
        {
            if (_contextDatabase == null)
            {
                _contextDatabase = new ContextDatabase<ContextItem>(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AICaloriesContextDatabase.db3"));
            }
            return _contextDatabase;
        }
    }

    public App(IServiceProvider serviceProvider)
	{
		InitializeComponent();
        MainPage = serviceProvider.GetRequiredService<AppShell>();

        ContextItemRepository = new ContextItemRepository(ContextDatabase);

        // Ensure ContextPage is initialized
        InitializeViewModels(serviceProvider);
    }

    private void InitializeViewModels(IServiceProvider serviceProvider)
    {
        var contextVM = serviceProvider.GetRequiredService<ContextVM>();
        var appSettingsVM = serviceProvider.GetRequiredService<AppSettingsVM>();
    }
}
