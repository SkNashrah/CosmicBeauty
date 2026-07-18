using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CosmicBeauty.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private string _userEmail;

        public HomeViewModel()
        {
            _userEmail = string.Empty;
        }
        public string UserEmail
        { 
            get => _userEmail;
            set
            { 
                if (SetProperty(ref _userEmail, value))
                { 
                    Notify(nameof(WelcomeMessage));
                }
            }
        }

        public string WelcomeMessage
        {
            get => $"Welcome {UserEmail}!";
        }
    }
}
