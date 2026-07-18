using CosmicBeauty.ViewModels;

namespace CosmicBeauty.Views;

public partial class CardView : ContentView
{
	public static readonly BindableProperty ImageProperty =
		BindableProperty.Create(nameof(Image), typeof(string), typeof(CardView), default(string));

	public static readonly BindableProperty NameProperty =
		BindableProperty.Create(nameof(Name), typeof(string), typeof(CardView), default(string));

	public static readonly BindableProperty SalePriceProperty =
		BindableProperty.Create(nameof(SalePrice), typeof(string), typeof(CardView), default(string));

	public static readonly BindableProperty ProductProperty =
		BindableProperty.Create(nameof(Product), typeof(ProductPageViewModel.Product), typeof(CardView), default(ProductPageViewModel.Product));

	

    public string Image
    {
        get => (string)GetValue(ImageProperty);
        set => SetValue(ImageProperty, value);
    }
    public string Name
	{
		get => (string)GetValue(NameProperty);
		set => SetValue(NameProperty, value);
	}

	public string SalePrice
	{
		get => (string)GetValue(SalePriceProperty);
		set => SetValue(SalePriceProperty, value);
	}

	public ProductPageViewModel.Product Product
	{
		get => (ProductPageViewModel.Product)GetValue(ProductProperty);
		set => SetValue(ProductProperty, value);
    }



	public CardView()
	{
		InitializeComponent();
	}
}