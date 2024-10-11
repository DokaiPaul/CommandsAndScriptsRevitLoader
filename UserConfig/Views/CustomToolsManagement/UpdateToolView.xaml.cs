using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Autodesk.Revit.DB;
using Autodesk.Windows;
using Microsoft.Win32;
using R2022.ButtonUtils;
using R2022.ENUM;
using ricaun.Revit.Mvvm;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Color = System.Windows.Media.Color;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;

namespace R2022.UserConfig.Views.CustomToolsManagement
{
    public partial class UpdateToolView : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public IRelayCommand CommandOpenFilePicker { get; private set; }
        public IRelayCommand CommandOpenImagePicker { get; private set; }
        public IRelayCommand CommandAddTool { get; private set; }
        public IRelayCommand CommandClose { get; private set; }

        private string _toolName;

        public string ToolName
        {
            get => _toolName;
            set
            {
                _toolName = value;
                OnPropertyChanged();
            }
        }

        private string _toolDescription;

        public string ToolDescription
        {
            get => _toolDescription;
            set
            {
                _toolDescription = value;
                OnPropertyChanged();
            }
        }

        private string _selectedFileType;

        public string SelectedFileType
        {
            get => _selectedFileType;
            set
            {
                _selectedFileType = value;
                OnPropertyChanged();
            }
        }

        private string _selectedFilePath;

        public string SelectedFilePath
        {
            get => _selectedFilePath;
            set
            {
                _selectedFilePath = value;
                OnPropertyChanged();
            }
        }

        private string _selectedImagePath;

        public string SelectedImagePath
        {
            get => _selectedImagePath;
            set
            {
                _selectedImagePath = value;
                OnPropertyChanged();
            }
        }


        public UpdateToolView(string toolName, string toolDescription, ToolTypes selectedFileType, string selectedFilePath,
            string selectedImagePath)
        {
            DataContext = this;

            CommandOpenFilePicker = new RelayCommand(OpenFilePicker);
            CommandOpenImagePicker = new RelayCommand(OpenImagePicker);
            CommandAddTool = new RelayCommand(AddTool);
            CommandClose = new RelayCommand(() => this.Close());

            InitializeComponent();
            InitializeWindow();

            SetThemeHandling();
            SetCloseWindowByKey();
            
            ToolName = toolName;
            ToolDescription = toolDescription;
            SelectedFileType = selectedFileType == ToolTypes.CSharp ? "C#" : "Dynamo";
            SelectedFilePath = selectedFilePath;
            SelectedImagePath = selectedImagePath;
        }

        private void InitializeWindow()
        {
            new WindowInteropHelper(this)
                { Owner = ComponentManager.ApplicationWindow };
        }

        private void OpenFilePicker()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Custom tools (*.dll, *.dyn)|*.dll;*.dyn",
                Title = "Select a file for custom tool."
            };

            if (openFileDialog.ShowDialog() != true) return;
            SelectedFilePath = openFileDialog.FileName;
            
            string fileExtension = Path.GetExtension(SelectedFilePath);
            switch (fileExtension)
            {
                case ".dll":
                    SelectedFileType = "C#";
                    break;
                case ".dyn":
                    SelectedFileType = "Dynamo";
                    break;
            }

        }

        private void OpenImagePicker()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "PNG files (*.png)|*.png",
                Title = "Select a 16x16 PNG image"
            };

            if (openFileDialog.ShowDialog() != true) return;
            string selectedFilePath = openFileDialog.FileName;

            // Check the dimensions of the selected image
            var bitmap = new BitmapImage(new Uri(selectedFilePath));
            if (bitmap.PixelWidth == 16 && bitmap.PixelHeight == 16)
            {
                SelectedImagePath = selectedFilePath;
            }
            else
            {
                // Show an error message or handle the invalid image size
                MessageBox.Show("Please select a PNG image with dimensions 16x16 pixels.", "Invalid Image Size",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void ApplicationThemeManager_Changed(ApplicationTheme currentApplicationTheme,
            Color systemAccent)
        {
            ApplicationThemeManager.Apply(this);
        }

        private void AddTool()
        {
            string errorMessage = String.Empty;

            if (String.IsNullOrEmpty(_toolName))
                errorMessage += "Tool name is required for a new tool.\n";
            if (_selectedFileType == null)
                errorMessage += "Please select a file type for the new tool.\n";
            if (String.IsNullOrEmpty(_selectedFilePath))
                errorMessage += "Please select a file for the new tool.\n";

            if (!String.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage, "Missing information", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            var configManager = new ConfigManager();
            try
            {
                ICustomButtonData newTool;
                if (_selectedFileType == "C#")
                {
                    newTool = new CSharpCustomButtonData(
                        _toolName,
                        _selectedFilePath,
                        "StartCommand",
                        _selectedImagePath,
                        _toolDescription
                    );
                }
                else
                {
                    newTool = new DynamoScriptCustomButtonData(
                        _toolName,
                        _selectedFilePath,
                        _selectedImagePath,
                        _toolDescription
                    );
                }

                configManager.AddNewTool(newTool);
                
                this.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Add the new tool to the database
            // ...

            // Close the window
            // this.Close();
        }

        #region Utilities

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}