using CosmicBeauty.ViewModels;
namespace CosmicBeauty.Views;

public partial class CheckoutPage : ContentPage
{
	public CheckoutPage(ProductPageViewModel productPageVm)
	{
        InitializeComponent();
		BindingContext = new CheckoutViewModel(productPageVm.BagItems);
    }
}