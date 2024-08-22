using AICalories.DI;
using AICalories.Models;
using AICalories.Repositories;
using AICalories.ViewModels;
using AICalories.Views;

namespace AICalories;


public partial class App : Application
{
    private static HistoryDatabase<MealItem> _historyDatabase;
    private static ContextDatabase<ContextItem> _contextDatabase;

    public static IHistoryItemRepository HistoryItemRepository { get; private set; }
    public static IContextItemRepository ContextItemRepository { get; private set; }


    public static HistoryDatabase<MealItem> HistoryDatabase
    {
        get
        {
            if (_historyDatabase == null)
            {
                _historyDatabase = new HistoryDatabase<MealItem>(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AICaloriesDatabase.db3")); //todo change to AICaloriesHistoryDatabase
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

        HistoryItemRepository = new HistoryItemRepository(HistoryDatabase);
        ContextItemRepository = new ContextItemRepository(ContextDatabase);

        // Ensure ContextPage is initialized
        InitializeViewModels(serviceProvider);
    }

    private void InitializeViewModels(IServiceProvider serviceProvider) //todo Try to use it
    {
        var contextVM = serviceProvider.GetRequiredService<ContextVM>();
        var appSettingsVM = serviceProvider.GetRequiredService<AppSettingsVM>();
    }
}
