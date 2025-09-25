using Microsoft.Maui.Controls;
using System.Text.RegularExpressions;
using BalanceUP.Data;
using BalanceUP.Models;
namespace BalanceUP.Pages
{
    public partial class SignUpPage : ContentPage
    {
        public SignUpPage()
        {
            InitializeComponent();
        }

        private async void OnCreateAccountClicked(object sender, EventArgs e)
        {
            // Verify input
            if (string.IsNullOrWhiteSpace(entryUsername.Text))
            {
                await DisplayAlert("Error", "Username is required.", "OK");
                return;
            }
            if (!IsValidEmail(entryEmail.Text))
            {
                await DisplayAlert("Error", "Please enter a valid email address.", "OK");
                return;
            }
            if (!IsValidPassword(entryPassword.Text))
            {
                await DisplayAlert("Error", "Password must be at least 6 characters long, " +
                    "contain at least one uppercase letter, one lowercase letter, " +
                    "and one special character.", "OK");
                return;
            }
            if (entryPassword.Text != entryConfirmPassword.Text)
            {
                await DisplayAlert("Error", "Passwords do not match.", "OK");
                return;
            }
            // Create a new user object
            UserInfo user = new UserInfo
            {
                Username = entryUsername.Text,
                Email = entryEmail.Text,
                Password = entryPassword.Text // Store the password
            };
            // Insert the user into the database
            DatabaseHelper databaseHelper = new DatabaseHelper();
            try
            {
                databaseHelper.InsertUser(user);
                Console.WriteLine($"User  '{user.Username}' created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating user: {ex.Message}");
                await DisplayAlert("Error", "Failed to create account. Please try again.", "OK");
                return;
            }
            // Set the current user
            UserInfo.Current = user; // Set the current user after account creation
            // success message and navigate to the login page
            await DisplayAlert("Account Created", "Your account has been created successfully!", "OK");
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }

        private async void OnSignInTapped(object sender, EventArgs e)
        {
            // Navigate back to the LoginPage
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }

        private void OnPasswordToggleTapped(object sender, EventArgs e)
        {
            // Password toggle
            entryPassword.IsPassword = !entryPassword.IsPassword;

            // Update the toggle
            passwordToggle.Text = entryPassword.IsPassword ? "Show" : "Hide";
        }

        private void OnConfirmPasswordToggleTapped(object sender, EventArgs e)
        {
            // Confirm password toggle
            entryConfirmPassword.IsPassword = !entryConfirmPassword.IsPassword;

            // Update the toggle
            confirmPasswordToggle.Text = entryConfirmPassword.IsPassword ? "Show" : "Hide";
        }

        private bool IsValidEmail(string email)
        {
            // Email validation
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
        }

        private bool IsValidPassword(string password)
        {
            // Password limitation
            return password.Length >= 6 &&
                   Regex.IsMatch(password, @"[A-Z]") && // At least one uppercase letter
                   Regex.IsMatch(password, @"[a-z]") && // At least one lowercase letter
                   Regex.IsMatch(password, @"[\W_]");   // At least one special character
        }
    }
}