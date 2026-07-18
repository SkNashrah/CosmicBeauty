using CosmicBeauty.ViewModels;
using System.Collections.ObjectModel;

namespace CosmicBeauty.Views
{
    public partial class AddProductForm : ContentPage
    {
        private readonly AddProductViewModel _viewModel;

        public AddProductForm(ObservableCollection<string> categories,
           ObservableCollection<ProductPageViewModel.Product> products )
        {
            InitializeComponent(); 
            _viewModel = new AddProductViewModel(categories, products);
            BindingContext = _viewModel;
        }

        private async void OnButtonPressed(object sender, EventArgs e)
        {
            var button = (Button)sender;
            await button.ScaleTo(0.8, 100, Easing.SinInOut);
        }

        private async void OnButtonReleased(object sender, EventArgs e)
        {
            var button = (Button)sender;
            await button.ScaleTo(1.0, 100, Easing.SinInOut);
        }
    }
}