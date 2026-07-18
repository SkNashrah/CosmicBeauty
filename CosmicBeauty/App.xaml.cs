using Microsoft.Extensions.DependencyInjection;
using CosmicBeauty;
using CosmicBeauty.Views;

namespace CosmicBeauty
{
    public partial class App : Application
    {
        public App()
        { 
            InitializeComponent();

            var navigationPage = new NavigationPage(new CosmicLoginPage())
            {
                // Set default colors (for when theme isn't specified)
                BarBackgroundColor = Color.FromArgb("#B99099"),
                BarTextColor = Colors.White
            };

            navigationPage.SetAppThemeColor(
                    NavigationPage.BarBackgroundColorProperty,
                    Color.FromArgb("#B99099"),  // Light theme
                    Color.FromArgb("#D38C9D")); // Dark theme

            navigationPage.SetAppThemeColor(
                NavigationPage.BarTextColorProperty,
                Colors.White,  // Light theme
                Colors.White); // Dark theme

            MainPage = navigationPage;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}