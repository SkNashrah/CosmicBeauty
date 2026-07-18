using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace CosmicBeauty.ViewModels
{
    public class AddProductViewModel : ViewModelBase
    {
        public ProductPageViewModel.Product NewProduct { get; } = new ProductPageViewModel.Product();
        public ObservableCollection<string> Categories { get; }
        private readonly ObservableCollection<ProductPageViewModel.Product> _products;

        private string _imageButtonText = "Select Image";
        public string ImageButtonText
        {
            get => _imageButtonText;
            set => SetProperty(ref _imageButtonText, value);
        }

        private string _fileContent;
        public string FileContent
        {
            get => _fileContent;
            set => SetProperty(ref _fileContent, value);
        }

        private string _selectTextFileButtonText = "Select Text File";
        public string SelectTextFileButtonText
        {
            get => _selectTextFileButtonText;
            set => SetProperty(ref _selectTextFileButtonText, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }
        private bool _hasFile;
        public bool HasFile
        {
            get => _hasFile;
            set => SetProperty(ref _hasFile, value);
        }
        private bool _hasImage;
        public bool HasImage
        {
            get => _hasImage;
            set => SetProperty(ref _hasImage, value);
        }

        public ICommand AddProductCommand { get; }
        public ICommand SelectImageCommand { get; }
        public ICommand SelectTextFileCommand { get; }
        public AddProductViewModel(ObservableCollection<string> categories,
                                   ObservableCollection<ProductPageViewModel.Product> products)
        {
            Categories = categories;
            _products= products;

            AddProductCommand = new Command(async () =>
            {
                await AddProduct();

            });
            SelectImageCommand = new Command(async () => await SelectImage());
            
            SelectTextFileCommand = new Command(async() => await SelectTextFile());

            RetryCommand = new Command(async () => await AddProduct());


            IsContentVisible = true;
        }


        private async Task AddProduct()
        {
            try
            {
             

                if (string.IsNullOrWhiteSpace(NewProduct.Name))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Name is required", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(NewProduct.OriginalPrice))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Original price is required", "OK");
                    return;
                }

                if (!decimal.TryParse(NewProduct.OriginalPrice.Replace("Rs ", ""), out _))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Please enter a valid price (numbers only)", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(NewProduct.Category))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Please select a category", "OK");
                    return;
                }

                if (!NewProduct.OriginalPrice.StartsWith("Rs "))
                    NewProduct.OriginalPrice = "Rs " + NewProduct.OriginalPrice;

                if (!NewProduct.SalePrice.StartsWith("Rs "))
                    NewProduct.SalePrice = "Rs " + NewProduct.SalePrice;


                var productToAdd = new ProductPageViewModel.Product
                {
                    Name = NewProduct.Name,
                    Category = NewProduct.Category,
                    OriginalPrice = NewProduct.OriginalPrice,
                    SalePrice = NewProduct.SalePrice,
                    Image = NewProduct.Image,
                    Brand = NewProduct.Brand, 
                    Description = this.Description
                };


                HasError = false;
                IsLoading = true;
                IsContentVisible = false;

                await Task.Delay(500);

                // Format prices

                _products.Add(productToAdd);

                // In a real app, you would save the product here
                await Application.Current.MainPage.DisplayAlert("Success", "Product added", "OK");

                NewProduct.Name = string.Empty;
                NewProduct.Category = string.Empty;
                NewProduct.OriginalPrice = string.Empty;
                NewProduct.SalePrice = string.Empty;
                NewProduct.Image = string.Empty;
                NewProduct.Description = string.Empty;
                ImageButtonText = "Select Image";
                HasImage = false;
                HasFile = false;

                await Application.Current.MainPage.Navigation.PopAsync();


            }
            catch (Exception ex)
            {
                HasError = true;
                Console.WriteLine($"Cosmic error: {ex.Message}");

            }
            finally
            {
                IsLoading = false;
            }
        }
        private async Task SelectImage()
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    NewProduct.Image = result.FullPath;
                    ImageButtonText = "Image Selected";
                    HasImage = true;
                    Notify(nameof(NewProduct));
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Image error: {ex.Message}", "OK");
            }
        }

        private async Task SelectTextFile()
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.Android, new[] { "text/plain", ".txt" } },
                        { DevicePlatform.iOS, new[] { "public.plain-text" } },
                        { DevicePlatform.WinUI, new[] { ".txt" } }
                    }),
                    PickerTitle = "Select a description file"
                });
                if (result == null) return;

                if (!result.FileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    await Shell.Current.DisplayAlert("Error", "Please select a .txt file", "OK");
                    HasFile = true;
                    return;
                }

                using var stream = await result.OpenReadAsync();
                using var reader = new StreamReader(stream);
                Description = await reader.ReadToEndAsync();
                SelectTextFileButtonText = "Description File Selected";
                NewProduct.Description = Description; // Also store in NewProduct
            }
            catch(Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"File Error:{ex.Message}", "OK");
            }
        }
    }
}