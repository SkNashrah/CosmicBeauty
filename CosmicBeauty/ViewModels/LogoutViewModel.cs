using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using CosmicBeauty.ViewModels;
using CosmicBeauty;
using CosmicBeauty.Views;

public class LogoutViewModel : ViewModelBase
{
    private readonly PopupViewModel _popupVm;

    public ICommand LogoutCommand { get; }
    public Action ConfirmLogout { get; set; }

    public LogoutViewModel()
    {

        // Set what happens when confirmed
        ConfirmLogout = () =>
        {
            Application.Current.MainPage = new NavigationPage(new CosmicLoginPage());
        };

        LogoutCommand = new Command(() =>
        {
            PopupViewModel.ShowConfirmationPopup(
                "Logout",
                "Are you sure?",
                ()=>
                {
                    Application.Current.MainPage = new NavigationPage(new CosmicLoginPage());
                });
        })
        {

        };
    }
}