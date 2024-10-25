using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AICalories.CustomControls;
using AICalories.DI;
using AICalories.Interfaces;
using AICalories.Models;
using AICalories.Services;

namespace AICalories.ViewModels
{
	public class ContextVM : INotifyPropertyChanged
    {
        private readonly IViewModelService _viewModelService;
        private readonly INavigationService _navigationService;
        private readonly IAlertService _alertService;
        private const string SelectedOptionKey = "SelectedOption";

        private string _selectedOption;
        private string _userInfo;
        private string _mainImage;
        private bool _isConfirmClicked;
        private IImageInfo _imageInfo;

        #region Properties

        public string SelectedOption
        {
            get => _selectedOption;
            set
            {
                if (_selectedOption != value)
                {
                    _selectedOption = value;
                    OnPropertyChanged();
                }
            }
        }


        public string UserInfo
        {
            get => _userInfo;
            set
            {
                if (_userInfo != value)
                {
                    _userInfo = value;
                    OnPropertyChanged();
                }
            }
        }
        //public string MainImage
        //{
        //    get => _imageInfo.ImagePath;
        //    set
        //    {
        //        if (_imageInfo.ImagePath != value)
        //        {
        //            _imageInfo.ImagePath = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        public string MainImage
        {
            get => _mainImage;
            set
            {
                if (_mainImage != value)
                {
                    _mainImage = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        public ICommand ConfirmCommand { get; }
        public ICommand NewImageCommand { get; }
        //public ICommand AddNewContextCommand { get; }



        public event Action<ContextGrid> ContextGridAdded;

        public ContextVM(IViewModelService viewModelService, IImageInfo imageInfo,
            INavigationService navigationService, IAlertService alertService)
        {
            _viewModelService = viewModelService;
            _viewModelService.ContextVM = this;
            _navigationService = navigationService;
            _alertService = alertService;

            _imageInfo = imageInfo;
            MainImage = _imageInfo.ImagePath;

            //AddNewContextCommand = new Command<VerticalStackLayout>(async (contextLayout) => await AddNewContextAsync(contextLayout));
            ConfirmCommand = new Command(async () => await OnConfirmAsync());
            NewImageCommand = new Command(async () => await OnNewImageAsync());

            LoadSelectedOption();


            //App.ContextDatabase.InsertContextItemAsync(new Models.ContextItem() { Text = "First", IsSelected = true });
            //App.ContextDatabase.InsertContextItemAsync(new Models.ContextItem() { Text = "Second", IsSelected = false });

        }
        public async Task SetAdditionalInfo()
        {
            _imageInfo.MealType = SelectedOption;
            _imageInfo.UserInfo = UserInfo;
        }

        private async Task LoadSelectedOption()
        {
            var savedOption = Preferences.Get(SelectedOptionKey, "This is a regular food");
            if (savedOption != null)
            {
                SelectedOption = savedOption;
            }
        }

        //private async Task AddNewContextAsync(VerticalStackLayout contextLayout)
        //{
        //    var newContextItem = new ContextItem() { Text = "First", IsSelected = true };
        //    await App.ContextItemRepository.InsertContextItemAsync(newContextItem);

        //    newContextItem = await App.ContextItemRepository.GetLastAddedContextItemAsync();
        //    var newContextGrid = new ContextGrid(newContextItem.Id, newContextItem.Text, newContextItem.IsSelected,
        //        App.ContextItemRepository, contextLayout);

        //    contextLayout.Children.Add(newContextGrid);
        //}


        private async Task AddNewContextAsync(VerticalStackLayout contextLayout)
        {
            var newContextItem = new ContextItem { Text = "First", IsSelected = true };
            await App.ContextItemRepository.InsertContextItemAsync(newContextItem);

            newContextItem = await App.ContextItemRepository.GetLastAddedContextItemAsync();

            var newContextGrid = new ContextGrid(newContextItem.Id, newContextItem.Text, newContextItem.IsSelected,
                App.ContextItemRepository, contextLayout);

            ContextGridAdded?.Invoke(newContextGrid);
        }


        public async Task UpdateContextList(VerticalStackLayout contextLayout)
        {
            contextLayout.Children.Clear();

            var items = await App.ContextDatabase.GetAllItemsAsync();

            foreach (var item in items)
            {
                var contextGrid = new ContextGrid(item.Id, item.Text, item.IsSelected,
                    App.ContextItemRepository, contextLayout);
                contextLayout.Children.Add(contextGrid);
            }
        }

        private async Task OnConfirmAsync()
        {
            if (_isConfirmClicked)
                return;

            _isConfirmClicked = true;

            await SetAdditionalInfo();

            if (!InternetConnection.CheckInternetConnection())
            {
                _alertService.ShowError("No internet connection");
                return;
            }

            _navigationService.PopModalAsync();
            await _navigationService.NavigateToResultPageAsync();

            _isConfirmClicked = false;
        }

        private async Task OnNewImageAsync()
        {
            _navigationService.PopModalAsync();
            await _navigationService.NavigateToTakeImagePageAsync();
        }

        //private void OnSetSelectedOption(object sender, CheckedChangedEventArgs e)
        //{
        //    if (e.Value)
        //    {
        //        var radioButton = sender as RadioButton;
        //        var selectedOption = radioButton?.Value?.ToString();
        //        // Save the selected option to preferences
        //        Preferences.Set(SelectedOptionKey, selectedOption);

        //        SelectedOption = selectedOption;
        //    }
        //}


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

