using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using CosmicBeauty.ViewModels;

namespace CosmicBeauty.Views
{
    public partial class CosmicLoginPage : ContentPage
    {
        private Popup currentPopup;

        private readonly SnackbarViewModel snackbarVm = new();

        public CosmicLoginPage()
        {
            InitializeComponent();
            BindingContext = new CosmicLoginViewModel(this, snackbarVm);
        }

        public async Task ShowCustomPopupAsync(Popup popup)
        {
            currentPopup = popup;
            await this.ShowPopupAsync(popup);
        }

        public async Task CloseCurrentPopupAsync()
        {
            if (currentPopup != null)
            {
                await currentPopup.CloseAsync();
                currentPopup = null;
            }
        }
    }
}