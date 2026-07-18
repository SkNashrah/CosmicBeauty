using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CosmicBeauty;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;


namespace CosmicBeauty.ViewModels
{
    public class CheckoutViewModel : ViewModelBase
    {
        private readonly PopupViewModel _popupVm;

        private string name;
        private string address;
        private string phone;
        private bool hasAddress;
        private bool showForm;
        private bool isBagEmpty;
        private string selectedPaymentMethod;

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }
        public string Address
        {
            get => address;
            set => SetProperty(ref address, value);
        }
        public string Phone
        {
            get => phone;
            set => SetProperty(ref phone, value);
        }
        public bool HasAddress
        {
            get => hasAddress;
            set
            {
                SetProperty(ref hasAddress, value);
                ShowForm = !value;
            }
        }
        public bool IsBagEmpty
        {
            get
            {
                if (BagItems.Count == 0)
                    return isBagEmpty = true;
                else
                    return isBagEmpty = false;
            }
            set => SetProperty(ref isBagEmpty, value);
        }
        public bool ShowForm
        {
            get => showForm;
            set => SetProperty(ref showForm, value);
        }

        
        public string SelectedPaymentMethod
        {
            get => selectedPaymentMethod;
            set => SetProperty(ref selectedPaymentMethod, value);
        }

        public decimal TotalPrice 
        {
            get
            {
                decimal total = 0;
                foreach (var item in BagItems)
                {
                    if (decimal.TryParse(item.SalePrice.Replace("Rs", ""), out decimal price))
                    {
                        total += price;
                    }
                }
                return total;
            }
        }

        public string TotalSavings
        {
            get
            {
                decimal totalSavings = 0;
                foreach (var item in BagItems)
                {
                    if (decimal.TryParse(item.SalePrice.Replace("Rs", ""), out decimal price) &&
                        decimal.TryParse(item.OriginalPrice.Replace("Rs", ""), out decimal originalPrice))
                    {
                        totalSavings += originalPrice - price;
                    }
                }
                return $"₹ {totalSavings}";
            }
        }
        public string DelivaryDate => DateTime.Now.AddDays(3).ToString("dddd, MMMM d");

        public ObservableCollection<ProductPageViewModel.Product> BagItems { get; } 

        public ICommand PlaceOrderCommand { get; }
        public ICommand UseSavedAddressCommand { get; }
        public ICommand SaveAddressCommand { get; }
        public ICommand AddNewAddressCommand { get; }

        public CheckoutViewModel(ObservableCollection<ProductPageViewModel.Product> bagItems)
        {
            BagItems = bagItems;
            ClearSavedAddress();
            HasAddress = Preferences.ContainsKey("SavedAddress");
            ShowForm = !HasAddress;

            

            if (HasAddress) 
            {
                Name = Preferences.Get("SavedName", "");
                Address = Preferences.Get("SavedAddress", "");
                Phone = Preferences.Get("SavedPhone", "");

            }
            PlaceOrderCommand = new Command(PlaceOrder);
            UseSavedAddressCommand = new Command(UseSavedAddress);
            SaveAddressCommand = new Command(SaveAddress);
            AddNewAddressCommand = new Command(() => ShowForm = true);
        }

        private void ClearSavedAddress()
        {
            Preferences.Remove("SavedName");
            Preferences.Remove("SavedAddress");
            Preferences.Remove("SavedPhone");
            HasAddress = false;
            ShowForm = true;
        }
        private void UseSavedAddress()
        {
            ShowForm = false;
        }

        private void SaveAddress()
        {
            Preferences.Set("SavedName", Name);
            Preferences.Set("SavedAddress", Address);
            Preferences.Set("SavedPhone", Phone);
            HasAddress = true;
        }
        private void PlaceOrder()
        {
            if (ShowForm)
            {
                if(string.IsNullOrWhiteSpace(Name) || 
                   string.IsNullOrWhiteSpace(Address) || 
                   string.IsNullOrWhiteSpace(Phone))
                {
                    Application.Current.MainPage.DisplayAlert(
                        "Error", 
                        "Please fill in all fields before placing an order.", 
                        "OK");
                    return;
                }
            }
            if (SelectedPaymentMethod == null)
            {
                Application.Current.MainPage.DisplayAlert(
                    "Error", 
                    "Please select a payment method.", 
                    "OK");
                return;
            }

            if (BagItems == null || BagItems.Count == 0)
            {
                Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Your bag is empty. Please add items before placing an order.",
                    "OK");
                return;
            }

            string message = $"Successfully!\n" +
                             $"Delivery By: {DelivaryDate}\n" +
                             $"Total Price: {TotalPrice}\n" +
                             $"You Saved: {TotalSavings}\n"+
                             $"Payment Method: {SelectedPaymentMethod}";
            PopupViewModel.ShowConfirmationPopup("Order Placed", message, () =>
            {
                BagItems.Clear();
                Application.Current.MainPage.Navigation.PopAsync();
            });
        }
    }
}
