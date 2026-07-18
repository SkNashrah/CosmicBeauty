using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using System.ComponentModel.DataAnnotations;
using CosmicBeauty.ViewModels;

namespace CosmicBeauty;

public class LoginViewModel : ViewModelBase
{
    //OLD CODE

    //public string Email { get; set; } = string.Empty;
    //public string Password { get; set; } = string.Empty;
    //public bool IsBusy { get; set; }
    public LoginViewModel()
    {
        _email = string.Empty;
        _password = string.Empty;
        _isBusy = false;
    }

    private string _email;
    private string _password;
    private bool _isBusy;   


    public string Email
    {
        get => _email;
            set
            {
                _email = value; //Update the backing field
                Notify(nameof(Email)); // Notify that Email property has changed
                Notify(nameof(EmailPreview));// Updates EmailPreview label on login page
                ((Command)LoginCommand)?.ChangeCanExecute(); //Login button state
            }
    }
    

    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            Notify(nameof(Password));
            Notify(nameof(PasswordPreview));
            ((Command)LoginCommand)?.ChangeCanExecute();
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (SetProperty(ref _isBusy, value))
            {
                ((Command)LoginCommand).ChangeCanExecute();
            }
        }
    }

    public string EmailPreview => !string.IsNullOrEmpty(Email) ? $"Email: {Email}" : "Email preview";

    public string PasswordPreview => !string.IsNullOrEmpty(Password) ? $"Password: {new string('•', Password.Length)} ({Password.Length}chars)" : "Password preview";

    public ICommand LoginCommand => new Command(execute: async () => await ExecuteLogin(), canExecute: () => !Validate());


    private async Task ExecuteLogin()
    {
        IsBusy = true;
        ((Command)LoginCommand).ChangeCanExecute();

        try
        {
            if (!Validate())
            {
                await Shell.Current.DisplayAlert("Error", "Invalid Email or Password", "OK");
                return;
            }

            await Shell.Current.GoToAsync($"//HomePage", new Dictionary<String, object>
            {
                {"UserEmail", Email }
            });
            Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;

        }
        finally
        {
            IsBusy = false;
        }
    }


    public bool Validate()
    {
        if (string.IsNullOrWhiteSpace(Email) || !IsValidEmail(Email))
        {
            return false;
        }
        if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6)
        {
            return false;
        }
        return true;
    }


    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

}

