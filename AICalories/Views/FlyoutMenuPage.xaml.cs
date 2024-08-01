using AICalories.ViewModels;

namespace AICalories.Views;

public partial class FlyoutMenuPage : ContentPage
{
    public ListView ListView;

    public FlyoutMenuPage()
	{
		InitializeComponent();

        BindingContext = new FlyoutMenuVM();
        //ListView = MenuItemsListView;
    }
}
