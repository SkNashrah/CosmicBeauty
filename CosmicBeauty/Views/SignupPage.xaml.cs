namespace CosmicBeauty.Views;

public partial class SignupPage : ContentPage
{
	public SignupPage()
	{

		   try 
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Initialization failed: {ex}");
            throw;
        }
    }

    private async void OnTermsTapped(object sender, EventArgs e)
	{
		// Simulate opening terms and conditions
		await DisplayAlert("Terms and Conditions", "Here are the terms and conditions...", "OK");
    }
    private async void OnSignupButtonClicked(object sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(UsernameEntry.Text)||string.IsNullOrWhiteSpace(EmailEntry.Text)||
			string.IsNullOrWhiteSpace(PasswordEntry.Text)||string.IsNullOrWhiteSpace(ConfirmPasswordEntry.Text))
		{
			await DisplayAlert("Error", "Please fill in all required fields.", "OK");
			return;
        }

		if(PasswordEntry.Text != ConfirmPasswordEntry.Text)
		{
			await DisplayAlert("Error", "Passwords do not match.", "OK");
			return;
        }

		if (!TermsConditions.IsChecked)
		{
			await DisplayAlert("Error", "You must agree to the terms and conditions.", "OK");
			return;
        }

            // Simulate a signup process
            await DisplayAlert("Signup successful!",
								$"Account Created for {UsernameEntry.Text}\n " +
								$"Birthday: {BirthdatePicker.Date:d}\n " +
								$"Birth Time: {BirthTimePicker.Time:t}\n " +
								$"Country: {CountryPicker.SelectedItem}", "OK");
    }
}