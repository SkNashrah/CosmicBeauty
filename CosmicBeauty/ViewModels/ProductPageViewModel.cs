using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CosmicBeauty.Views;

namespace CosmicBeauty.ViewModels
{
    public class ProductPageViewModel : ViewModelBase
    {
        private readonly LogoutViewModel _logoutVm;
        public class Product : ViewModelBase
        {
            public string Name { get; set; }
            public string Category { get; set; }
            public string OriginalPrice { get; set; }
            public string SalePrice { get; set; }
            public string Image { get; set; }
            public string Brand { get; set; }
            public string Description { get; set; }
            public string Review { get; set; }

            private bool isInWishlist;
            public bool IsInWishlist
            {
                get => isInWishlist;
                set => SetProperty(ref isInWishlist, value);
            }

            private int stock;
            public int Stock
            {
                get => stock;
                set
                {
                    if (SetProperty(ref stock, value))
                    {
                        // Update the display properties when stock changes
                        StockStatus = value <= 10 ? $"Only {value} left" : "";
                        ShowStockWarning = value <= 10 && value > 0;
                    }
                }
            }

            private string stockStatus;
            public string StockStatus
            {
                get => stockStatus;
                set => SetProperty(ref stockStatus, value);
            }

            private bool showStockWarning;
            public bool ShowStockWarning
            {
                get => showStockWarning;
                set => SetProperty(ref showStockWarning, value);
            }

            
        }


        private bool _isMenuVisible;
        public bool IsMenuVisible
        {
            get => _isMenuVisible;
            set => SetProperty(ref _isMenuVisible, value);
        }

        public class MenuItem
        {
            public string Title { get; set; }
            public ICommand Command { get; set; }
        }

       
        public string Username { get; }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private int stock;
        public int Stock 
        {
            get => stock;
            set
            {
                if (SetProperty(ref stock, value))
                {
                    Notify(nameof(StockStatus));
                    Notify(nameof(ShowStockWarning));
                }
            }
        }

        public string StockStatus => Stock <= 10 ? $"{Stock} left" : "";
        public bool ShowStockWarning => Stock <= 10;



        public ObservableCollection<Product> Products { get; } = new ObservableCollection<Product>();
        public ObservableCollection<string> Categories { get; } = new ObservableCollection<string>();
        public ObservableCollection<MenuItem> MenuItems { get; } = new ObservableCollection<MenuItem>();
        public static ObservableCollection<Product> AllProducts { get; } = new ObservableCollection<Product>();
        public ObservableCollection<Product> BagItems { get; } = new ObservableCollection<Product>();

        private readonly SnackbarViewModel _snackbarVm = new();

        private static WishlistViewModel _wishlistVm;
        public static WishlistViewModel WishlistVm => _wishlistVm;


        public ICommand ToggleMenuCommand { get; }
        public ICommand DeleteProductCommand { get; }
        public ICommand NavigateToDetailsCommand { get; }
        public ICommand NavigateToAddProductCommand { get; }
        public ICommand NavigateToSettingsCommand { get; }
        public ICommand WishlistCommand { get; }
        public ICommand NavigateToBagCommand { get; }
        public ICommand AddToBagCommand { get; }


        public ProductPageViewModel(string username,LogoutViewModel logoutVm)
        {
            IsMenuVisible = false;

            AddToBagCommand = new Command<Product>(AddToBag);
            _wishlistVm = new WishlistViewModel(AddToBagCommand);

            Username = username;
            _logoutVm = logoutVm;


            ToggleMenuCommand = new Command(ToggleMenu);
            DeleteProductCommand = new Command<Product>(DeleteProduct);
            NavigateToDetailsCommand = new Command<Product>(NavigateToDetails);
            NavigateToAddProductCommand = new Command(NavigateToAddProduct);
            NavigateToSettingsCommand = new Command(NavigateToSettings);
            WishlistCommand = new Command<Product>(AddToWishlist);
            NavigateToBagCommand = new Command(async () => await NavigateToBag());


            // Update navigation commands to hide menu
            NavigateToSettingsCommand = new Command(async () =>
            {
                IsMenuVisible = false;
                await Application.Current.MainPage.Navigation.PushAsync(new SettingsPage());
            });

            NavigateToAddProductCommand = new Command(async () =>
            {
                IsMenuVisible = false;
                await Application.Current.MainPage.Navigation.PushAsync(new AddProductForm(Categories, Products));
            });

           

            InitializeMenu();
            LoadData();


            CheckProductStockLevel();
        }

        private void CheckProductStockLevel()
        {
            foreach (var product in Products)
            {
                if (product.Stock <= 10)
                {
                    // You can implement additional logic here, such as notifying the user
                    Console.WriteLine($"Warning: {product.Name} stock is low ({product.Stock} left)");
                }
            }
        }



        private void ToggleMenu()
        {
            IsMenuVisible = !IsMenuVisible;
            Console.WriteLine($"Menu toggled. Visible: {IsMenuVisible}");
        }
        private void InitializeMenu()
        {
            MenuItems.Clear();

            MenuItems.Add(new MenuItem
            {
                Title = "Wishlist",
                Command = new Command(() =>
                {
                    IsMenuVisible = false;
                    Application.Current.MainPage.Navigation.PushAsync(new WishlistPage());
                })
            });
            MenuItems.Add(new MenuItem
            {
                Title = "Add New Product",
                Command = NavigateToAddProductCommand
            });
            MenuItems.Add(new MenuItem
            {
                Title = "Settings",
                Command = NavigateToSettingsCommand
            });

            MenuItems.Add(new MenuItem
            {
                Title = "Logout",
                Command = _logoutVm.LogoutCommand
            });
        }

        private async void DeleteProduct(Product product)
        {
            if (product == null) return;
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirm Delete",
                $"Delete {product.Name}?",
                "Delete", "Cancel");
            if (confirm) Products.Remove(product);
        }

        private async void NavigateToDetails( Product product)
        {

            if (product == null) return;
            try
            {
                await ShowLoading();
                await Task.Delay(1000);
                await Application.Current.MainPage.Navigation.PushAsync(
                    new DetailPage(Username, product, this));
            }
            finally
            {
                await HideLoading();
            }
        }

        private async void NavigateToAddProduct()
        {
            try { 
                await ShowLoading();
                await Task.Delay(300);
                await Application.Current.MainPage.Navigation.PushAsync(
                    new AddProductForm(Categories, Products));
            }
            finally
            {
                await HideLoading();
            }
        }
        private async void NavigateToSettings()
        {
            try
            { 
                await ShowLoading();
                await Task.Delay(300);
                IsMenuVisible = false;
                await Application.Current.MainPage.Navigation.PushAsync(new SettingsPage());
            }
            finally
            {
                await HideLoading();
            }
        }


        private async void AddToWishlist(Product product)
        {
            if (product != null)
            {
                var existingItem = WishlistVm.WishlistItems.FirstOrDefault(item =>
                     item.Name == product.Name &&
                     item.Category == product.Category &&
                     item.Brand == product.Brand);

                if (existingItem == null)
                {
                    product.IsInWishlist = true;
                    WishlistVm.WishlistItems.Add(product);
                    await _snackbarVm.ShowSnackbar($"{product.Name} added to wishlist", "#4CAF50");
                }
                else
                {
                    await _snackbarVm.ShowSnackbar($"{product.Name} is already in wishlist", "#FF5252");
                }
            }
        }

        private async Task NavigateToBag()
        {
            try
            {
                await ShowLoading();
                await Task.Delay(100);
                IsMenuVisible = false;
                await Application.Current.MainPage.Navigation.PushAsync(new CheckoutPage(this));
            }
            finally
            {
                await HideLoading();
            }
        }


        private async void AddToBag(Product product)
        {
            if (product != null && !BagItems.Contains(product))
            {
                BagItems.Add(product);
                await _snackbarVm.ShowSnackbar($"{product.Name} added to cart", "green");
            }
        }

        private async Task LoadData()
        {
            try
            {
                await ShowLoading();
                await Task.Delay(50);
                if (Products.Count > 0)
                {
                    return;
                }


                // Load categories
                Categories.Clear();
                Categories.Add("All");
                Categories.Add("Skincare");
                Categories.Add("Makeup");
                Categories.Add("Haircare");
                Categories.Add("Fragrance");


                    // Load products
                    Products.Add(new Product
                    {
                        Name = "Glow-Blush",
                        Category = "Makeup",
                        OriginalPrice = "Rs 2000",
                        SalePrice = "Rs 1500",
                        Image = "pixiblush.jpg",
                        Brand = "Pixi",
                        Stock = 50,
                        Description = "A beautiful blush that gives you a natural glow."
                    });

                    Products.Add(new Product
                    {
                        Name = "SuperStay Foundation",
                        Category = "Makeup",
                        OriginalPrice = "Rs 800",
                        SalePrice = "Rs 650",
                        Image = "superstayfoundation.jpg",
                        Brand = "Maybelline",
                        Stock = 90,
                        Description = "A long-lasting foundation that stays put all day."
                    });

                    Products.Add(new Product
                    {
                        Name = "Glow Screen",
                        Category = "Skincare",
                        OriginalPrice = "Rs 2500",
                        SalePrice = "Rs 2000",
                        Image = "supergoopsunscreen.jpg",
                        Brand = "Supergoop",
                        Stock = 10,
                        Description = "A sunscreen that gives you a radiant glow while protecting your skin."
                    });

                    Products.Add(new Product
                    {
                        Name = "Dreamy Length Shampoo",
                        Category = "Haircare",
                        OriginalPrice = "Rs 1200",
                        SalePrice = "Rs 700",
                        Image = "lorealshampoo.jpg",
                        Brand = "L'Oreal",
                        Stock = 7,
                        Description = "A shampoo that nourishes and strengthens your hair for dreamy lengths."
                    });

                    Products.Add(new Product
                    {
                        Name = "62 Perfume",
                        Category = "Fragrance",
                        OriginalPrice = "Rs 3000",
                        SalePrice = "Rs 2500",
                        Image = "sdjbodymist.jpg",
                        Brand = "Sol de Janeiro",
                        Stock = 48,
                        Description = "A luxurious body mist that leaves you smelling divine."
                    });

                    Products.Add(new Product
                    {
                        Name = "Essence Oil",
                        Category = "Skincare",
                        OriginalPrice = "Rs 1800",
                        SalePrice = "Rs 1500",
                        Image = "pixiessenceoil.jpg",
                        Brand = "Pixi",
                        Stock = 3,
                        Description = "A soothing essential oil that relaxes your mind and body."
                    });

                    Products.Add(new Product
                    {
                        Name = "Foaming Cleanser",
                        Category = "Skincare",
                        OriginalPrice = "Rs 1000",
                        SalePrice = "Rs 800",
                        Image = "neutrogenafacewash.jpg",
                        Brand = "Neutrogen",
                        Stock = 4,
                        Description = "A Gentle foaming for all type of skin"
                    });

                    Products.Add(new Product
                    {
                        Name = "Eyeshadows",
                        Category = "Makeup",
                        OriginalPrice = "Rs 2500",
                        SalePrice = "Rs 2000",
                        Image = "revolutioneyepallete.jpg",
                        Brand = "Makeup Revolution",
                        Stock = 50,
                        Description = "A versatile eyeshadow palette with a range of stunning colors."
                    });

                    Products.Add(new Product
                    {
                        Name = "Niacinamide Serum",
                        Category = "Skincare",
                        OriginalPrice = "Rs 1400",
                        SalePrice = "Rs 1100",
                        Image = "ordinaryniacinamide.jpg",
                        Brand = "The Ordinary",
                        Stock = 69,
                        Description = "This serum reduce inflamations dark spots and controls oil."
                    });

                    Products.Add(new Product
                    {
                        Name = "Radiance deodrant",
                        Category = "Fragrance",
                        OriginalPrice = "Rs 900",
                        SalePrice = "Rs 700",
                        Image = "dovedeo.jpg",
                        Brand = "Dove",
                        Stock = 37,
                        Description = "A refreshing deodorant that keeps you feeling fresh all day."
                    });

                    Products.Add(new Product
                    {
                        Name = "Hair Serum",
                        Category = "Haircare",
                        OriginalPrice = "Rs 1600",
                        SalePrice = "Rs 1300",
                        Image = "lorealhairserum.jpg",
                        Brand = "L'Oreal",
                        Stock =78,
                        Description = "A nourishing hair serum that adds shine and smoothness to your hair."
                    });

                foreach(var product in Products)
                {
                    product.IsInWishlist = WishlistVm.WishlistItems.Any(item =>
                            item.Name == product.Name &&
                            item.Category == product.Category &&
                            item.Brand == product.Brand);
                }
            }
            finally
            {
                await HideLoading();
            }
        }
    }
}