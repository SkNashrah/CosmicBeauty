using CommunityToolkit.Maui.Views;
using System.Windows.Input;

namespace CosmicBeauty.Views
{
    public partial class ConfirmationPopup : Popup
    {
        //public string Title { get; }
        //public string Message { get; }
        //public Action OnConfirm { get; set; }

        //public ICommand CancelCommand { get; }
        //public ICommand ConfirmCommand { get; }

        public ConfirmationPopup()
        {
            InitializeComponent();
            //Title = title;
            //Message = message;
            //OnConfirm = onConfirm;

            //CancelCommand = new Command(() => CloseAsync());
            //ConfirmCommand = new Command(() =>
            //{
            //    OnConfirm?.Invoke();
            //    CloseAsync();
            //    // Handle confirmation directly here
            //    (BindingContext as LogoutViewModel)?.ConfirmLogout();
            //});

            //BindingContext = this;
        }
    }
}