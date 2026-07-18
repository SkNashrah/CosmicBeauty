using CosmicBeauty.ViewModels;

namespace CosmicBeauty.Views
{
    public partial class DetailPage : ContentPage 
    {
        private readonly ProductDetailViewModel _viewModel;

        public DetailPage(string username, ProductPageViewModel.Product product, ProductPageViewModel productPageVM)
        {
            InitializeComponent(); 
            _viewModel = new ProductDetailViewModel(username, product, productPageVM);
            BindingContext = _viewModel;
        }

        protected override void OnDisappearing()
        {
            if (_viewModel.Review != null)
            {
                _viewModel.CurrentProduct.Review = _viewModel.Review;

            }
            base.OnDisappearing();
        }
    }
}