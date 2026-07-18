using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using CosmicBeauty.Views;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CosmicBeauty.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        private bool isLoading;
        private bool hasError;
        private bool isContentVisible;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void Notify([CallerMemberName] string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            Notify(propName);
            return true;
        }

        private string _currentState;
        public string CurrentState
        {
            get => _currentState;
            private set => SetProperty(ref _currentState, value);
        }

        public bool IsLoading
        {
            get => isLoading;
            set
            {
                SetProperty(ref isLoading, value);
                UpdateState();
            }
        }

        public bool HasError
        {
            get => hasError;
            set
            {
                SetProperty(ref hasError, value);
                UpdateState();
            }
        }

        public bool IsContentVisible
        {
            get => isContentVisible;
            set
            {
                SetProperty(ref isContentVisible, value);
                UpdateState();
            }
        }

        protected void UpdateState()
        {
            if (IsLoading)
                CurrentState = "Loading";
            else if (HasError)
                CurrentState = "Error";

            else
                CurrentState = "Content";
        }


        public ICommand RetryCommand { get; protected set; }

        private LoadingPage _loadingPage;

        protected async Task ShowLoading()
        {
            IsLoading = true;
            IsContentVisible = false;
            _loadingPage = new LoadingPage();
            await Application.Current.MainPage.Navigation.PushModalAsync(_loadingPage);
            await Task.Delay(100); 
        }

        protected async Task HideLoading()
        {
            if (_loadingPage != null)
            {
                await Application.Current.MainPage.Navigation.PopModalAsync();
                _loadingPage = null;
            }
            IsLoading = false;
            IsContentVisible = true;
            await Task.Delay(100); 
        }

        protected async Task ExecuteWithLoading(Func<Task> action)
        {
            if(IsLoading)
            {
                await action();
                return;
            }
            try
            {
                hasError = false;
                if(!IsLoading)
                {
                    await ShowLoading();

                }
                await action();
            }
            catch (Exception ex)
            {
                HasError = true;
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
            finally
            {
                if (IsLoading)
                    await HideLoading();
            }

        }
    }
}
