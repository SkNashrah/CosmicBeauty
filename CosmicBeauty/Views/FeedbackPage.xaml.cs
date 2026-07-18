using CosmicBeauty.ViewModels;
namespace CosmicBeauty.Views;

public partial class FeedbackPage : ContentPage
{
	public FeedbackPage(FeedbackViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}