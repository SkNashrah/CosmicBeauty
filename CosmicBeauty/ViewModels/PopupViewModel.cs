using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CosmicBeauty.Views;

namespace CosmicBeauty.ViewModels
{
    public class PopupViewModel : ViewModelBase
    {
        private string _title;
        private string _message;
        private readonly Action _onConfirm;
        private Popup _popup;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        public PopupViewModel(string title, string message, Action onConfirm, Popup popup)
        {
            Title = title;
            Message = message;
            _onConfirm = onConfirm;
            _popup = popup;

            ConfirmCommand = new Command(Confirm);
            CancelCommand = new Command(Cancel);
        }

        private async void Confirm()
        {
            if(_popup != null)
            {
                await _popup.CloseAsync();
            }
            _onConfirm?.Invoke();
        }

        private async void Cancel()
        {
            if (_popup != null)
            {
                await _popup.CloseAsync();
            }
        }

        public static void ShowConfirmationPopup(string title, string message, Action onConfirm)
        {
            var popup = new ConfirmationPopup();
            var popupViewModel = new PopupViewModel(title, message, onConfirm, popup);
            popup.BindingContext = popupViewModel;
            if (Application.Current?.MainPage != null)
            {
                Application.Current.MainPage.ShowPopup(popup);
            }
        }
    }
}