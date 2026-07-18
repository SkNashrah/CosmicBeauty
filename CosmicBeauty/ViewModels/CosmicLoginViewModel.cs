using System.Windows.Input;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using CosmicBeauty.Views;

namespace CosmicBeauty.ViewModels
{
    public class CosmicLoginViewModel : ViewModelBase
    {
        private readonly CosmicLoginPage page;
        private readonly SnackbarViewModel snackbarVm;
        private string email = string.Empty;
        private string password = string.Empty;
        private bool isBusy;
        private string passwordError;
        private bool showPasswordError;
        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }

        public string PasswordError
        {
            get => passwordError;
            set
            {
                if (SetProperty(ref passwordError, value))
                {
                    ShowPasswordError = !string.IsNullOrEmpty(value);
                }
            }
        }

        public bool ShowPasswordError
        {
            get => showPasswordError;
            set => SetProperty(ref showPasswordError, value);
        }

        public ICommand LoginCommand { get; }
        

        public CosmicLoginViewModel(CosmicLoginPage page, SnackbarViewModel snackbarVm)
        {
            this.page = page;
            this.snackbarVm = snackbarVm;
            LoginCommand = new Command(async () => await Login());
           
        }

        private async Task Login()
        {
            PasswordError = null;
            bool hasError = false;

            if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6)
            {
                PasswordError = "Password must be 6+ characters";
                hasError = true;
            }

            if (hasError) return;


            try
            {
                await ExecuteWithLoading (async ()=>
                {
                    string username = Email.Split('@')[0];
                    await snackbarVm.ShowSnackbar($"Welcome!, {username}!", "#4CAF50");
                    await Task.Delay(250);

                    await Application.Current.MainPage.Navigation.PushAsync(new ProductPage(username));
                });
            }
            catch (Exception ex)
            {
                await snackbarVm.ShowSnackbar($"Navigation Failed, Error: {ex.Message}", "#F44336");
            }
            finally
            {
                IsBusy = false;
            }

        }

       

       
    }
}


//using CommunityToolkit.Maui.Core;
//using CommunityToolkit.Maui.Views;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.Runtime.CompilerServices;
//using System.Windows.Input;
//using CommunityToolkit.Maui.Alerts;

//namespace CosmicBeauty.ViewModels
//{
//    public class CosmicLoginViewModel : INotifyPropertyChanged
//    {
//        private readonly CosmicLoginPage _page;
//        private string _email = string.Empty;
//        private string _password = string.Empty;
//        private bool _isBusy;

//        public event PropertyChangedEventHandler PropertyChanged;

//        public CosmicLoginViewModel(CosmicLoginPage page)
//        {
//            _page = page;
//            LoginCommand = new Command(async () => await Login(), () => CanLogin());
//            ConfirmLoginCommand = new Command(async () => await ConfirmLogin());
//            CancelLoginCommand = new Command(async () => await CancelLogin());
//        }

//        public string Email
//        {
//            get => _email;
//            set
//            {
//                if (_email != value)
//                {
//                    _email = value;
//                    OnPropertyChanged();
//                    ((Command)LoginCommand).ChangeCanExecute();
//                }
//            }
//        }

//        public string Password
//        {
//            get => _password;
//            set
//            {
//                if (_password != value)
//                {
//                    _password = value;
//                    OnPropertyChanged();
//                    ((Command)LoginCommand).ChangeCanExecute();
//                }
//            }
//        }

//        public bool IsBusy
//        {
//            get => _isBusy;
//            set
//            {
//                if (_isBusy != value)
//                {
//                    _isBusy = value;
//                    OnPropertyChanged();
//                    ((Command)LoginCommand).ChangeCanExecute();
//                }
//            }
//        }

//        public ICommand LoginCommand { get; }
//        public ICommand ConfirmLoginCommand { get; }
//        public ICommand CancelLoginCommand { get; }

//        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
//        {
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//        }

//        private async Task ConfirmLogin()
//        {
//            await ExecuteLogin();
//            await ClosePopupAsync();
//        }

//        private async Task CancelLogin()
//        {
//            await ClosePopupAsync();
//        }

//        private async Task ClosePopupAsync()
//        {
//            await _page.CloseCurrentPopupAsync();
//        }
//        private async Task Login()
//        {
//            if (IsBusy) return;

//            var popup = new ConfirmationPopup
//            {
//                BindingContext = this
//            };

//            await _page.ShowCustomPopupAsync(popup);
//        }


//        private async Task ExecuteLogin()
//        {
//            if (IsBusy) return;

//            try
//            {
//                IsBusy = true;
//                await Task.Delay(1000);

//                if (!IsValidEmail(Email))
//                {
//                    await ShowSnackbar("Invalid email format", Colors.OrangeRed);
//                    return;
//                }

//                if (Password.Length < 6)
//                {
//                    await ShowSnackbar("Password must be 6+ characters", Colors.OrangeRed);
//                    return;
//                }

//                await ShowSnackbar($"Welcome, {Email.Split('@')[0]}!", Colors.Green);
//                await Task.Delay(1500);

//                await MainThread.InvokeOnMainThreadAsync(async () =>
//                {
//                    await Application.Current.MainPage.Navigation.PushAsync(new ProductPage());
//                });
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine($"Error: {ex}");
//                await ShowSnackbar($"Error: {ex.Message}", Colors.Red);
//            }
//            finally
//            {
//                IsBusy = false;
//            }
//        }

//        private async Task ShowSnackbar(string message, Color bgColor)
//        {
//            try
//            {
//                var page = Application.Current?.MainPage ??
//                    throw new InvalidOperationException("No current page found");

//                var options = new SnackbarOptions
//                {
//                    BackgroundColor = bgColor,
//                    TextColor = Colors.White,
//                    CornerRadius = new CornerRadius(12),
//                    Font = Microsoft.Maui.Font.SystemFontOfSize(14, FontWeight.Bold),
//                    ActionButtonTextColor = Colors.White,
//                };

//                await Snackbar.Make(
//                    message: message,
//                    action: () => { },
//                    actionButtonText: "OK",
//                    duration: TimeSpan.FromSeconds(3),
//                    visualOptions: options,
//                    anchor: page)
//                    .Show();
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine($"Snackbar error: {ex}");
//#if DEBUG
//                MainThread.BeginInvokeOnMainThread(async () =>
//                {
//                    await Application.Current?.MainPage?.DisplayAlert("Error", ex.Message, "OK");
//                });
//#endif
//            }
//        }

//        private bool CanLogin() => !string.IsNullOrWhiteSpace(Email) &&
//                                 !string.IsNullOrWhiteSpace(Password);

//        private bool IsValidEmail(string email)
//        {
//            if (string.IsNullOrWhiteSpace(email))
//                return false;

//            return email.Contains("@") &&
//                   email.IndexOf("@") < email.LastIndexOf(".") &&
//                   email.Length > 5;
//        }
//    }
//}