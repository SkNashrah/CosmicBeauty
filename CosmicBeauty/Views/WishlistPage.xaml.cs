using CosmicBeauty.ViewModels;


namespace CosmicBeauty.Views;
public partial class WishlistPage : ContentPage
{
	public WishlistPage()
	{
		InitializeComponent();
		BindingContext = ProductPageViewModel.WishlistVm;

		if(BindingContext is WishlistViewModel viewModel)
		{
			viewModel.Title = "Wishlist";
        }
    }


}