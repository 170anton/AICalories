using AICalories.ViewModels;

namespace AICalories.Views;

public partial class SupportPage : ContentPage
{
    private SupportVM _viewModel;

	public SupportPage()
	{
		InitializeComponent();
        _viewModel = new SupportVM();
        BindingContext = _viewModel;
            
        editor.SizeChanged += OnEditorSizeChanged;
    }



    private void OnEditorSizeChanged(object sender, EventArgs e)
    {
        var editor = sender as Editor;
        if (editor != null)
        {
            var frame = editor.Parent as Frame;
            if (frame != null)
            {
                // Update frame's height based on editor's height
                frame.HeightRequest = editor.Height;
            }
        }
    }

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("//history");
        return true;
    }
}
