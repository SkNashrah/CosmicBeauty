using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using CosmicBeauty.ViewModels;


namespace CosmicBeauty.ViewModels
{
    public class FeedbackViewModel : ViewModelBase
    {
        public ProductPageViewModel.Product CurrentProduct { get; }
        public string ProductName => CurrentProduct.Name;
        public string ProductImage => CurrentProduct.Image;
        public string ProductDescription => CurrentProduct.Description;

        private string comment;
        private int rating;
        private string[] starGlyphs = new string[5];
        public int Rating
        {
            get => rating;
            set
            {
                if (SetProperty(ref rating, value))
                {
                    UpdateStarGlyphs();
                }
            }
        }

        public string Comment
        {
            get => comment;
            set => SetProperty(ref comment, value);
        }

        public string[] StarGlyphs
        {
            get => starGlyphs;
            private set => SetProperty(ref starGlyphs, value);
        }
        public ICommand SetRatingCommand { get; }
        public ICommand SubmitFeedbackCommand { get; }

        public event Action<ReviewItem> FeedbackSubmitted;
        public FeedbackViewModel(ProductPageViewModel.Product product)
        {
            CurrentProduct = product;
            StarGlyphs = new string[5] { "☆", "☆", "☆", "☆", "☆" };
            Notify(nameof(StarGlyphs));
            SetRatingCommand = new Command<object>(param =>
            {
                if (param is int starIndex )
                {
                    OnStarTapped(starIndex);
                }
                else if (param is string starString && int.TryParse(starString, out int index) && index >= 1 && index <= 5)
                {
                    OnStarTapped(index - 1);
                }
               
            });
            SubmitFeedbackCommand = new Command(OnSubmittedFeedback);

            UpdateStarGlyphs();
        }

        private void OnStarTapped(int starIndex)
        {
            Debug.WriteLine($"Star tapped: {starIndex + 1}");
            Rating = starIndex + 1;
            UpdateStarGlyphs();
        }
        private void UpdateStarGlyphs()
        {
            Console.WriteLine($"Updating stars for rating: {Rating}");
            var newGlyphs = new string[5] ;
            for (int i = 0; i < 5; i++)
            {
                newGlyphs[i] = (i + 1) <= Rating ? "★" : "☆";
            }
            StarGlyphs = newGlyphs;
            Notify(nameof(StarGlyphs));
        }

        private void OnSubmittedFeedback()
        {
            if (Rating == 0)
            {
                Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Please select a rating before submitting.",
                    "OK");
                return;
            }
            else if (string.IsNullOrWhiteSpace(comment))
            {
                Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Please enter a comment before selecting a rating.",
                    "OK");
            }


            else
            {
                var review = new ReviewItem
                {
                    UserName = "You",
                    UserImage = "",
                    ProductName = ProductName,
                    ProductImage = ProductImage,
                    ReviewDate = DateTime.Now,
                    Comment = Comment,
                    Rating = Rating
                };

                FeedbackSubmitted?.Invoke(review);
                Application.Current.MainPage.DisplayAlert(
                    "Review Submitted",
                    "Thank you for your feedback!",
                    "OK");
                Application.Current?.MainPage?.Navigation?.PopAsync();
            }
        }

    }

    public class ReviewItem
    {
        public string UserName { get; set; }
        public string UserImage { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public DateTime ReviewDate { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }

        public string StarRating
        {
            get
            {
                return new string('★', Rating) + new string('☆', 5 - Rating);
            }
        }
    }
}
