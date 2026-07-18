using CosmicBeauty.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;


namespace CosmicBeauty.Views
{
    public partial class ProductPage : ContentPage // Add 'partial' keyword
    {
        public ProductPage(string username)
        {
            InitializeComponent();

            var logoutVm = new LogoutViewModel();
            BindingContext = new ProductPageViewModel(username, logoutVm);
            Preferences.Set("username", username);
        }

        private async void OnProductSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is ProductPageViewModel.Product selectedProduct)
            {
                string username = Preferences.Get("username", string.Empty);
                var productPageVM = BindingContext as ProductPageViewModel;
                await Navigation.PushAsync(new DetailPage(username, selectedProduct, productPageVM));
                ((CollectionView)sender).SelectedItem = null;
            }
        }

        private void OnWishlistPressed(object sender, EventArgs e)
        {
            productsCollectionView.SelectionMode = SelectionMode.None;
        }

        private async void OnWishlistReleased(object sender, EventArgs e)
        {
            await Task.Delay(100); 
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                productsCollectionView.SelectionMode = SelectionMode.Single;
            });
        }

        private async void OnAddToBagPressed(object sender, EventArgs e)
        {
            productsCollectionView.SelectionMode = SelectionMode.None;
        }
        private async void OnAddToBagReleased(object sender, EventArgs e)
        {
            await Task.Delay(100); 
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                productsCollectionView.SelectionMode = SelectionMode.Single;
            });
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is ProductPageViewModel vm)
            {
                vm.IsMenuVisible = false;
            }
        }

        //private void OnWishlistTapped(object sender, EventArgs e)
        //{
        //    if (sender is View view && view.BindingContext is Product product)
        //    {
        //        // Execute the wishlist command
        //        if (BindingContext is ProductPageViewModel viewModel)
        //        {
        //            viewModel.WishlistCommand.Execute(product);
        //        }
        //    }
        //}
    }
}


//namespace CosmicBeauty;

//public partial class ProductPage : ContentPage
//{
//    public ProductPage()
//    {
//        InitializeComponent();
//        BindingContext = new ProductPageViewModel();
//        NavigationPage.SetHasBackButton(this, false);
//    }

//    private async void OnProductSelected(object sender, SelectionChangedEventArgs e)
//    {
//        var selectedProduct = e.CurrentSelection.FirstOrDefault() as ProductViewModel.Product;

//        if (selectedProduct != null)
//        {
//            ((CollectionView)sender).SelectedItem = null;

//            await Navigation.PushAsync(new DetailPage
//            {
//                BindingContext = selectedProduct
//            });
//        }


//    }
//}