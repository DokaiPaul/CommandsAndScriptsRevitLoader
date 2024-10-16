using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Autodesk.Revit.UI;
using Autodesk.Windows;
using R2022.Types.ENUM;
using R2022.Utils;
using R2022.Utils.Buttons;
using ricaun.Revit.Mvvm;
using Wpf.Ui;
using Wpf.Ui.Appearance;

namespace R2022.UserConfig.Views.CustomToolsManagement
{
    public partial class MainCustomToolsView : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public IRelayCommand CommandAddNewTool { get; private set; }

        private ObservableCollection<ToolItem> _currentItems;

        public ObservableCollection<ToolItem> CurrentItems
        {
            get => _currentItems;
            set
            {
                _currentItems = value;
                OnPropertyChanged();
            }
        }
        
        public MainCustomToolsView()
        {
            DataContext = this;
            
            SetCommandsHandlers();

            InitializeComponent();
            InitializeWindow();

            PopulateCustomToolItems();

            SetThemeHandling();
            SetCloseWindowByKey();
        }

        private void InitializeWindow()
        {
            _ = new WindowInteropHelper(this)
                { Owner = ComponentManager.ApplicationWindow };
        }


        private void SetCommandsHandlers()
        {
            CommandAddNewTool = new RelayCommand(OpenAddNewToolDialog);
        }


        private void PopulateCustomToolItems()
        {
            CurrentItems = new ObservableCollection<ToolItem>();

            var configManager = new ConfigManager();

            var cSharpItems = configManager.GetCustomCsharpTools();
            foreach (CSharpCustomButtonData item in cSharpItems)
            {
                CurrentItems.Add(new ToolItem
                {
                    Id = item.Id,
                    Name = item.ButtonName,
                    Description = item.Description,
                    FilePath = item.FilePath,
                    IconPath = item.ButtonImagePath,
                    Type = item.ToolType
                });
            }

            var dynamoItems = configManager.GetCustomDynamoTools();
            foreach (DynamoScriptCustomButtonData item in dynamoItems)
            {
                CurrentItems.Add(new ToolItem
                {
                    Id = item.Id,
                    Name = item.ButtonName,
                    Description = item.Description,
                    FilePath = item.FilePath,
                    IconPath = item.ButtonImagePath,
                    Type = item.ToolType
                });
            }
        }

        private void SetThemeHandling()
        {
            var configManager = new ConfigManager();
            string stringTheme = configManager.GetTheme();
            ApplicationTheme theme = stringTheme == "Light"
                ? ApplicationTheme.Light
                : ApplicationTheme.Dark;

            ApplicationThemeManager.Apply(theme);
            ApplicationThemeManager.Apply(this);

            ApplicationThemeManager.Changed += ApplicationThemeManager_Changed;
            this.Unloaded += (s, e) => { ApplicationThemeManager.Changed -= ApplicationThemeManager_Changed; };
        }

        private void SetCloseWindowByKey()
        {
            this.KeyDown += (s, e) =>
            {
                if (e.Key == Key.Escape)
                {
                    this.Close();
                }
            };
        }

        #region Handlers

        private void OpenAddNewToolDialog()
        {
            var window = new AddNewToolView();
            window.ShowDialog();

            PopulateCustomToolItems();
        }

        private void EditTool_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button) || !(button.DataContext is ToolItem toolItem)) return;

            var window = new UpdateToolView(toolItem.Id, toolItem.Name, toolItem.Description, toolItem.Type,
                toolItem.FilePath, toolItem.IconPath);
            window.ShowDialog();

            PopulateCustomToolItems();
        }

        private void DeleteTool_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button) || !(button.DataContext is ToolItem toolItem)) return;

            MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete the tool: {toolItem.Name}?",
                "Delete Tool", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            var configManager = new ConfigManager();

            try
            {
                configManager.RemoveTool(toolItem.Id, toolItem.Type);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            PopulateCustomToolItems();
        }

        #endregion

        private void ApplicationThemeManager_Changed(ApplicationTheme currentApplicationTheme,
            Color systemAccent)
        {
            ApplicationThemeManager.Apply(this);
        }


        #region Utilities

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public class ToolItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public string IconPath { get; set; }
        public ToolTypes Type { get; set; }
    }
}