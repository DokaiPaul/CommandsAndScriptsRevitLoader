using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Autodesk.Windows;
using ricaun.Revit.Mvvm;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace R2022.UserConfig.Views.DisplaySettings
{
    public partial class UpdateDisplaySettingsView
    {
        public IRelayCommand CommandChangeTheme { get; private set; }


        public UpdateDisplaySettingsView()
        {
            DataContext = this;

            SetCommandsHandlers();

            InitializeComponent();
            InitializeWindow();

            SetThemeHandling();
            SetCloseWindowByKey();
        }


        private void ApplicationThemeManager_Changed(ApplicationTheme currentApplicationTheme,
            Color systemAccent)
        {
            ApplicationThemeManager.Apply(this);
        }

        private void UpdateTheme()
        {
            string stringTheme;
            ApplicationTheme theme;
            if (ApplicationThemeManager.GetAppTheme() == ApplicationTheme.Light)
            {
                theme = ApplicationTheme.Dark;
                stringTheme = "Dark";
            }
            else
            {
                theme = ApplicationTheme.Light;
                stringTheme = "Light";
            }

            ApplicationThemeManager.Apply(theme);

            var configManager = new ConfigManager();
            configManager.SetTheme(stringTheme);
        }

        private void SetCommandsHandlers()
        {
            CommandChangeTheme = new RelayCommand(UpdateTheme);
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


        #region InitializeWindow

        private void InitializeWindow()
        {
            new WindowInteropHelper(this)
                { Owner = ComponentManager.ApplicationWindow };
        }

        #endregion
    }
}