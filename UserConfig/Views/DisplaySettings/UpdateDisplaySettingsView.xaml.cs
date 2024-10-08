using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using R2022.ButtonUtils;
using ricaun.Revit.Mvvm;
using Wpf.Ui.Appearance;

namespace R2022.UserConfig.Views.DisplaySettings
{
    public partial class UpdateDisplaySettingsView
    {
        public IRelayCommand CommandChangeTheme { get; private set; }
        

        public UpdateDisplaySettingsView()
        {
            CommandChangeTheme = new RelayCommand(UpdateTheme);


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
        

        private void ApplicationThemeManager_Changed(ApplicationTheme currentApplicationTheme,
            System.Windows.Media.Color systemAccent)
        {
            ApplicationThemeManager.Apply(this);
        }
        
        private void UpdateTheme()
        {
            var theme = ApplicationThemeManager.GetAppTheme() != ApplicationTheme.Light
                ? ApplicationTheme.Light
                : ApplicationTheme.Dark;
            ApplicationThemeManager.Apply(theme);
        }


        #region InitializeWindow

        private void InitializeWindow()
        {
            new System.Windows.Interop.WindowInteropHelper(this)
            { Owner = Autodesk.Windows.ComponentManager.ApplicationWindow };
        }

        #endregion
    }
}