using Microsoft.Maui.Storage;
using System.Windows.Input;

namespace CosmicBeauty.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private bool _darkMode;
        public bool DarkMode
        {
            get => _darkMode;
            set => SetProperty(ref _darkMode, value);
        }

        private bool _enableNotifications;
        public bool EnableNotifications
        {
            get => _enableNotifications;
            set => SetProperty(ref _enableNotifications, value);
        }

        private double _volumeLevel = 50;
        public double VolumeLevel
        {
            get => _volumeLevel;
            set
            {
                if (SetProperty(ref _volumeLevel, value))
                {
                    VolumeProgress = value / 100;
                }
            }
        }

        private double _volumeProgress;
        public double VolumeProgress
        {
            get => _volumeProgress;
            set => SetProperty(ref _volumeProgress, value);
        }

        public ICommand SaveCommand { get; }

        public SettingsViewModel()
        {
            // Load saved settings
            DarkMode = Preferences.Get("DarkMode", Application.Current.RequestedTheme == AppTheme.Dark);
            EnableNotifications = Preferences.Get("EnableNotifications", false);
            VolumeLevel = Preferences.Get("VolumeLevel", 50.0);
            VolumeProgress = VolumeLevel / 100;

            PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(DarkMode))
                {
                    ApplyTheme();
                }
            };

            SaveCommand = new Command(SaveSettings);
        }

        private void ApplyTheme()
        {
            Application.Current.UserAppTheme = DarkMode ? AppTheme.Dark : AppTheme.Light;
        }


        private void SaveSettings()
        {
            Preferences.Set("DarkMode", DarkMode);
            Preferences.Set("EnableNotifications", EnableNotifications);
            Preferences.Set("VolumeLevel", VolumeLevel);

            // Apply theme
            Application.Current.UserAppTheme = DarkMode ? AppTheme.Dark : AppTheme.Light;

            Application.Current.MainPage.DisplayAlert("Success", "Settings saved", "OK");
        }
    }
}