using CosmicBeauty.Models;
using CosmicBeauty.PageModels;

namespace CosmicBeauty.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}