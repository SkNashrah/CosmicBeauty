using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using CosmicBeauty.Views;

namespace CosmicBeauty.ViewModels
{
    class ProductViewModel : INotifyPropertyChanged
    {

      
        public class Product
        {
            public string Name { get; set; }
            public string Category { get; set; }
            public string OriginalPrice { get; set; }
            public string SalePrice { get; set; }
            public string Image { get; set; }
            public string Brand { get; set; }
            public string Description { get; set; }
        }

        public class MenuItem
        {
            public string Title { get; set; }
            public ICommand Command { get; set; }
        }

        private bool _isMenuVisible;
        public bool IsMenuVisible
        {
            get => _isMenuVisible;
            set
            {
                if (_isMenuVisible != value)
                {
                    _isMenuVisible = value;
                    OnPropertyChanged(nameof(IsMenuVisible));
                }
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged(nameof(IsBusy));
                }
            }
        }

        // Add to ProductViewModel class
        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged(nameof(SelectedProduct));
            }
        }

        public string ImageButtonText { get; set; } = "Select Image";
        public Product NewProduct { get; set; } = new Product();

        // Collections
        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<string> Categories { get; set; }
        public ObservableCollection<MenuItem> MenuItems { get; set; }

        // Commands
        public ICommand AddProductCommand { get; private set; }
        public ICommand SelectImageCommand { get; private set; }
        public ICommand ToggledMenuCommand { get; }

        public ICommand ProductSelectedCommand { get; }
        public ICommand AddNewProductCommand => new Command(async () =>
        {
            await ShowLoadingandNavigate(async () =>
            {
                await Task.Delay(500);
                //await Application.Current.MainPage.Navigation.PushAsync(new AddProductForm
                //{
                //    BindingContext = this
                //});
            });

            //Application.Current.MainPage.Navigation.PushAsync(new AddProductForm
            //{
            //    BindingContext = this
            //});
        });
        public ICommand ViewDetailsCommand => new Command<Product>(async (product) =>
        {
            await Application.Current.MainPage.DisplayAlert("Details",
                $"Name: {product.Name}\nPrice: {product.OriginalPrice}", "OK");
        });
        public ICommand DeleteProductCommand => new Command<Product>(async (product) =>
        {
            if (product == null)
            {
                await ShowError("Product not found for deletion.");
                return;
            }

            bool confirm = await Application.Current.MainPage.DisplayAlert("Confirm Delete",
                $"Are you sure you want to delete {product.Name}?", "Delete", "Cancel");

            if (confirm)
            {
                Products.Remove(product);
                await Application.Current.MainPage.DisplayAlert("Success", $"{product.Name} has been deleted.", "OK");
            }
        });
        public ICommand NavigateToDetailsCommand => new Command<Product>(async (product) =>
        {
            if (product == null)
            {
                return;
            }

            await ShowLoadingandNavigate(async () =>
            {
                await Task.Delay(700);
                //await Application.Current.MainPage.Navigation.PushAsync(new DetailPage
                //{
                //    BindingContext = product
                //});
            });

            //await Application.Current.MainPage.Navigation.PushAsync(new DetailPage
            //{
            //    BindingContext = product
            //});
        });

        public ICommand NavigateToSettingsCommand => new Command(async () =>
        {
            await ShowLoadingandNavigate(async () =>
            {
                await Task.Delay(500);
                await Application.Current.MainPage.Navigation.PushAsync(new SettingsPage());
                IsMenuVisible = false;
            });
        });


        public ICommand WishlistCommand { get; set; }
        private async void ExecuteWishlistCommand(Product product)
        {
            if (product != null)
            {
                await Application.Current.MainPage.DisplayAlert("Wishlist",
                    $"{product.Name} added to wishlist", "OK");
            }
        }

        // Events
        public event PropertyChangedEventHandler PropertyChanged;

        // Constructor
        public ProductViewModel()
        {
            Products = new ObservableCollection<Product>();
            Categories = new ObservableCollection<string>();
            ProductSelectedCommand = new Command<Product>(OnProductSelected);
            WishlistCommand = new Command<Product>(ExecuteWishlistCommand);


            ToggledMenuCommand = new Command(() =>
            {
                IsMenuVisible = !IsMenuVisible;
            });


            MenuItems = new ObservableCollection<MenuItem>
            {
                new MenuItem { Title= "Add New Product", Command= AddNewProductCommand},
                new MenuItem { Title= "Settings", Command = NavigateToSettingsCommand }
            };


            try
            {
                LoadCategories();
                LoadProducts();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ViewModel initialization error: {ex}");
                // Optionally reinitialize collections if needed
                if (Categories == null) Categories = new ObservableCollection<string>();
                if (Products == null) Products = new ObservableCollection<Product>();
            }


            AddProductCommand = new Command(async () => await AddProduct());
            SelectImageCommand = new Command(async () => await SelectImage());

        }
      
        private void OnProductSelected(Product product) { }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Private methods
        private async Task AddProduct()
        {
            if (!IsValidProduct(NewProduct))
            {
                await ShowError("Please fill all required fields");
                return;
            }

            if (string.IsNullOrWhiteSpace(NewProduct.Name))
            {
                await ShowError("Product name cannot be empty.");
                return;
            }

            if (!NewProduct.OriginalPrice.StartsWith("Rs"))
            {
                NewProduct.OriginalPrice = "Rs " + NewProduct.OriginalPrice;
            }

            if (!NewProduct.SalePrice.StartsWith("Rs"))
            {
                NewProduct.SalePrice = "Rs " + NewProduct.SalePrice;
            }

            Products.Add(NewProduct);
            NewProduct = new Product();
            ImageButtonText = "Select Image";
            OnPropertyChanged(nameof(ImageButtonText));

            await Application.Current.MainPage.DisplayAlert("Success", "Product added successfully!", "OK");
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private async Task SelectImage()
        {
            try
            {
                var options = new PickOptions
                {
                    PickerTitle = "Select an Image",
                    FileTypes = FilePickerFileType.Images
                };

                var result = await FilePicker.PickAsync(options);
                if (result != null)
                {
                    NewProduct.Image = result.FullPath;
                    ImageButtonText = "Image Selected";
                    OnPropertyChanged(nameof(ImageButtonText));
                }
                else
                {
                    await ShowError("No image selected.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Image selection error: {ex}");
                await ShowError("Failed to select image.");
            }
        }

        private async Task ShowLoadingandNavigate(Func<Task> navigationAction)
        {
            var loadingPage = new LoadingPage();
            await Application.Current.MainPage.Navigation.PushModalAsync(loadingPage);

            try
            {
                await navigationAction();
            }
            finally
            {
                await Application.Current.MainPage.Navigation.PopModalAsync();
            }
        }

        private bool IsValidProduct(Product product)
        {
            return !string.IsNullOrWhiteSpace(product.Name) &&
                   !string.IsNullOrWhiteSpace(product.Category) &&
                   IsValidPrice(product.OriginalPrice) &&
                   IsValidPrice(product.SalePrice) &&
                   !string.IsNullOrWhiteSpace(product.Image) &&
                   !string.IsNullOrWhiteSpace(product.Brand) &&
                   !string.IsNullOrWhiteSpace(product.Description);
        }

        private bool IsValidPrice(string price)
        {
            return price.StartsWith("Rs ") && decimal.TryParse(price.Substring(3), out _);
        }


        private async Task ShowError(string message)
        {
            await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
        }

        private async Task LoadCategories()
        {
            IsBusy = true;
            try
            {
                await Task.Delay(5000);
                Categories.Clear();

                Categories.Add("All");
                Categories.Add("Skincare");
                Categories.Add("Makeup");
                Categories.Add("Haircare");
                Categories.Add("Fragrance");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Category load error: {ex}");
                throw;
            }
        }

        private async Task LoadProducts()
        {
            IsBusy = true;
            try
            {
                await Task.Delay(500);
                Products.Clear();

                Products.Add(new Product
                {
                    Name = "Glow-Blush",
                    Category = "Makeup",
                    OriginalPrice = "Rs 2000",
                    SalePrice = "Rs 1500",
                    Image = "pixiblush.jpg",
                    Brand = "Pixi",
                    Description = "A beautiful blush that gives you a natural glow."
                });

                Products.Add(new Product
                {
                    Name = "SuperStay Foundation",
                    Category = "Makeup",
                    OriginalPrice = "Rs 1500",
                    SalePrice = "Rs 1200",
                    Image = "superstayfoundation.jpg",
                    Brand = "Maybelline",
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
                    Description = "A sunscreen that gives you a radiant glow while protecting your skin."
                });

                Products.Add(new Product
                {
                    Name = "Dreamy Length Shampoo",
                    Category = "Haircare",
                    OriginalPrice = "Rs 1200",
                    SalePrice = "Rs 1000",
                    Image = "lorealshampoo.jpg",
                    Brand = "L'Oreal",
                    Description = "A shampoo that nourishes and strengthens your hair for dreamy lengths."
                });

                Products.Add(new Product
                {
                    Name = "62 Perfume",
                    Category = "Fragrance",
                    OriginalPrice = "Rs 3000",
                    SalePrice = "Rs 2800",
                    Image = "sdjbodymist.jpg",
                    Brand = "Sol de Janeiro",
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
                    Description = "A nourishing hair serum that adds shine and smoothness to your hair."
                });
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}