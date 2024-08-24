using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AICalories.Interfaces;
using AICalories.Models;
using AICalories.Repositories;

namespace AICalories.CustomControls
{
    public class ContextGrid : Grid, IContextItem, INotifyPropertyChanged
    {

        private readonly IContextItemRepository _contextItemRepository;
        private int _id;
        private string _text;
        private bool _isSelected;
        private VerticalStackLayout _parentLayout;

        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        
        public ContextGrid(int id, string text, bool isSelected,
            IContextItemRepository contextItemRepository, VerticalStackLayout parentLayout) : base()
        {
            Id = id;
            Text = text;
            IsSelected = isSelected;
            _contextItemRepository = contextItemRepository;
            _parentLayout = parentLayout;

            InitializeGrid();
            
        }


        private void InitializeGrid()
        {
            Padding = new Thickness(0);

            var column0 = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) };
            var column1 = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }; // * sizing
            var column2 = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) }; // Auto sizing

            this.ColumnDefinitions.Add(column0);
            this.ColumnDefinitions.Add(column1);
            this.ColumnDefinitions.Add(column2);

            //var dropdownMenu = new DropdownMenu
            //{
            //    //Padding = 0,
            //    //BackgroundColor = Colors.Transparent,
            //    //BorderColor = Colors.Transparent,
            //    //TextColor = Colors.Black,
            //    //FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
            //    //Text = "⋮", //"✖"

            //};
            var showOptionsButton = new Button
            {
                Text = "⋮",
                BackgroundColor = Colors.Transparent,
                BorderColor = Colors.Transparent,
                TextColor = Colors.Black,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
            };

            var textLabel = new Label
            {
                Text = Text,
                HorizontalTextAlignment = TextAlignment.Start,
                Style = Application.Current?.Resources["LabelAppStyle"] as Style
            };

            var checkBox = new CheckBox { IsChecked = IsSelected };

            Grid.SetColumn(showOptionsButton, 0);
            Grid.SetColumn(textLabel, 1);
            Grid.SetColumn(checkBox, 2);

            //optionsButton.AddMenuItem("Delete", OnOption1Clicked);

            this.Children.Add(showOptionsButton);
            this.Children.Add(textLabel);
            this.Children.Add(checkBox);

            //overflowButton.Clicked += async (sender, e) =>
            //{
            //    await DeleteFromDatabaseAsync(parentLayout);
            //};

            showOptionsButton.Clicked += async (sender, e) =>
            {
                // Show an ActionSheet with multiple options
                string action = await App.Current.MainPage.DisplayActionSheet("Select an option", null, null, "Update", "Delete");

                // You can handle the selected option here
                if (action == "Update")
                {
                    await App.Current.MainPage.DisplayAlert("Selected", "You selected Option 1", "OK");
                }
                else if (action == "Delete")
                {
                    await App.Current.MainPage.DisplayAlert("Selected", "You selected Option 2", "OK");
                }
            };

            checkBox.CheckedChanged += async (sender, e) =>
            {
                IsSelected = e.Value;
                await SaveToDatabaseAsync();
            };

        }

        private void OnOption1Clicked(object sender, EventArgs e)
        {
            DeleteFromDatabaseAsync(_parentLayout);
        }

        private async Task DeleteFromDatabaseAsync(VerticalStackLayout parentLayout)
        {
            var contextItem = ToContextItem();

            await _contextItemRepository.DeleteContextItemAsync(contextItem);

            parentLayout.Children.Remove(this);
        }

        //private async Task DeleteFromDatabaseAsync(VerticalStackLayout parentLayout)
        //{
        //    var contextItem = ToContextItem();

        //    await _contextItemRepository.DeleteContextItemAsync(contextItem);

        //    parentLayout.Children.Remove(this);
        //}

        private async Task SaveToDatabaseAsync()
        {
            var contextItem = ToContextItem();

            await _contextItemRepository.UpdateContextItemAsync(contextItem);
        }

        private ContextItem ToContextItem()
        {
            return new ContextItem
            {
                Id = this.Id,
                Text = this.Text,
                IsSelected = this.IsSelected
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}

