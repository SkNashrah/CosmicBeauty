using Microsoft.Maui.Controls;
using CosmicBeauty.ViewModels;

namespace CosmicBeauty.Views
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            BindingContext = new SettingsViewModel();
        }
    }
}