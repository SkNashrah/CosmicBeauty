using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.Input;
using CosmicBeauty.Views;

namespace CosmicBeauty.ViewModels
{
    public class WishlistViewModel:ViewModelBase
    {
        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public ObservableCollection<ProductPageViewModel.Product> WishlistItems { get; } = new();

        public ICommand RemoveCommand { get; }
        public ICommand AddToBagCommand { get; }
        public ICommand LoadWishlistCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand SettingsCommand { get; }


        private readonly SnackbarViewModel _snackbarVm = new();

        public WishlistViewModel(ICommand addToBagCommand)
        {
            RemoveCommand = new Command<ProductPageViewModel.Product>(async (product) => await Remove(product));

            AddToBagCommand = new Command<ProductPageViewModel.Product>(async (product) =>
            {
                if (product != null && addToBagCommand != null)
                {
                    addToBagCommand.Execute(product);
                    await _snackbarVm.ShowSnackbar($"{product.Name} added to bag", "#4CAF50");
                }
            });

            LoadWishlistCommand = new Command(async () => await LoadWishlist());

            GoBackCommand = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PopAsync();
            });

            SettingsCommand = new Command(async () =>
            {
                 await Application.Current.MainPage.Navigation.PushAsync(new SettingsPage());
            });

        }

        private async Task Remove(ProductPageViewModel.Product product)
        {
            if (product != null)
            {
                var itemToRemove = WishlistItems.FirstOrDefault(item =>
                    item.Name == product.Name &&
                    item.Category == product.Category &&
                    item.Brand == product.Brand);

                if (itemToRemove != null)
                {
                    WishlistItems.Remove(itemToRemove);
                    itemToRemove.IsInWishlist = false;
                    await _snackbarVm.ShowSnackbar($"{product.Name} removed from wishlist", "#FF5252");
                }
            }
        }
        private async Task LoadWishlist()
        {
            await Task.CompletedTask;
        }

       

    }


}

