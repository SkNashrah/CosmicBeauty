using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using CosmicBeauty.Views;

namespace CosmicBeauty.ViewModels
{
    public class ProductDetailViewModel : ViewModelBase
    {
        private readonly SnackbarViewModel _snackbarVm = new();
        private static WishlistViewModel _wishlistVm;
        public static WishlistViewModel WishlistVm => _wishlistVm;
        private readonly ProductPageViewModel productPageViewModel;
        public ProductPageViewModel.Product CurrentProduct { get; }
        private string _review;
        public string Review
        {
            get => _review;
            set => SetProperty(ref _review, value);
        }
        public ICommand WishlistCommand => productPageViewModel.WishlistCommand;
        public ICommand AddToBagCommand => productPageViewModel.AddToBagCommand;
        public ICommand NavigateFeedbackCommand { get; set; }

        private ObservableCollection<ReviewItem> reviews;
        public ObservableCollection<ReviewItem> Reviews
        {
            get => reviews;
            set => SetProperty(ref reviews, value);
        }

        public ProductDetailViewModel(string username, ProductPageViewModel.Product product, ProductPageViewModel productPageVM)
        {
            CurrentProduct = product;
            username = username;
            Review = product.Review;
            productPageViewModel = productPageVM;
            
            NavigateFeedbackCommand = new Command(async () =>
            {
                var feedbackVm = new FeedbackViewModel(CurrentProduct);
                feedbackVm.FeedbackSubmitted += OnFeedbackSubmitted;
                await Application.Current.MainPage.Navigation.PushAsync(new FeedbackPage(feedbackVm));
            });
            Reviews = new ObservableCollection<ReviewItem>();
        }
        private void OnFeedbackSubmitted(ReviewItem review)
        {
            Reviews.Insert(0, review);
        }
      
        public class ReviewItems : ViewModelBase
        {
            public string UserName { get; set; }
            private string userImage = "userplaceholderpink.jpeg";
            public string UserImage
            {
                get => userImage;
                set => SetProperty(ref userImage, value);
            }

            public string ProductName { get; set; }
            public string ProductImage { get; set; }
            public DateTime ReviewDate { get; set; }
            public string Comment { get; set; }
            public int Rating { get; set; }

            
        }
    }
}