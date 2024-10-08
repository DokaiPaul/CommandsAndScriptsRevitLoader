using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using R2022.ButtonUtils;
using ricaun.Revit.Mvvm;
using Wpf.Ui.Appearance;

namespace R2022.UserConfig.Views.CustomToolsManagement
{
    public partial class MainCustomToolsView : INotifyPropertyChanged
    {
        // colors for some buttons
        public string PlanitMainColor { get; set; } = "#5F749F";
        public Brush AddItemBackground { get; set; } = Brushes.DarkSeaGreen;
        public Brush EditItemBackground { get; set; } = Brushes.DarkGoldenrod;
        public Brush DeleteItemBackground { get; set; } = Brushes.PaleVioletRed;


        public event PropertyChangedEventHandler PropertyChanged;


        public ObservableCollection<ToolItem> DynamoItems { get; set; }
        public ObservableCollection<ToolItem> CSharpItems { get; set; }
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
            // Set DataContext
            DataContext = this;

            // get preset items from preset for C# tools
            var configManager = new ConfigManager();

            var cSharpItems = configManager.GetCustomCsharpTools();
            CSharpItems = new ObservableCollection<ToolItem>();
            foreach (CSharpButtonData item in cSharpItems)
            {
                CSharpItems.Add(new ToolItem { Name = item.ButtonName, Description = item.Description });
            }

            var dynamoItems = configManager.GetCustomDynamoTools();
            DynamoItems = new ObservableCollection<ToolItem>();
            foreach (DynamoScriptButtonData item in dynamoItems)
            {
                DynamoItems.Add(new ToolItem { Name = item.ButtonName, Description = item.Description });
            }

            InitializeComponent();
            InitializeWindow();

            ApplicationThemeManager.Apply(this);
            ApplicationThemeManager.Changed += ApplicationThemeManager_Changed;
            this.Unloaded += (s, e) => { ApplicationThemeManager.Changed -= ApplicationThemeManager_Changed; };

            this.KeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Escape)
                {
                    this.Close();
                }
            };
        }

        #region Handlers

        public void ToggleToolsCommand(object sender, RoutedEventArgs routedEventArgs)
        {
            CurrentItems = CurrentItems == CSharpItems
                ? DynamoItems
                : CSharpItems;
            OnPropertyChanged(nameof(CurrentItems));
        }

        public void EditCommand()
        {
            return;
        }

        public void DeleteCommand()
        {
            return;
        }

        #endregion

        private void ApplicationThemeManager_Changed(ApplicationTheme currentApplicationTheme,
            System.Windows.Media.Color systemAccent)
        {
            ApplicationThemeManager.Apply(this);
        }


        #region InitializeWindow

        private void InitializeWindow()
        {
            new System.Windows.Interop.WindowInteropHelper(this)
                { Owner = Autodesk.Windows.ComponentManager.ApplicationWindow };

            // Set initial items
            CurrentItems = DynamoItems;
        }

        #endregion

        #region Utilities

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public class ToolItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}