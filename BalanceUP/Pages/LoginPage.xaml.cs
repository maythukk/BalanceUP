using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using BalanceUP.Data;
using BalanceUP.Models;

namespace BalanceUP.Pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            LoadSavedCredentials(); // Call the method to load saved credentials
        }

        private async void LoadSavedCredentials()
        {
            // Load saved username if "Remember Me" was checked
            string savedUsername = Preferences.Get("username", string.Empty);
            entryUsername.Text = savedUsername;

            // fetch the password securely if "Remember Me" was checked
            if (!string.IsNullOrWhiteSpace(savedUsername))
            {
                try
                {
                    string savedPassword = await SecureStorage.GetAsync("password");
                    entryPassword.Text = savedPassword;
                }
                catch (Exception ex)
                {
                    // Handle exceptions
                    Console.WriteLine($"Error retrieving password: {ex.Message}");
                }
            }
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            try
            {
                string username = entryUsername?.Text ?? string.Empty;
                string password = entryPassword?.Text ?? string.Empty;

                Console.WriteLine($"Username: {username}, Password: {password}");

                // Check if fields are not empty
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    await DisplayAlert("Login Failed", "Please enter a valid username and password.", "OK");
                    return;
                }

                // Verify user credentials
                DatabaseHelper databaseHelper = new DatabaseHelper();
                UserInfo user = databaseHelper.GetUserByUsername(username);

                // Log the retrieved user data
                if (user != null)
                {
                    Console.WriteLine($"Retrieved User: {user.Username}, Email: {user.Email}"); // Log the retrieved user
                }
                else
                {
                    Console.WriteLine($"No user found with username: {username}"); // Log if no user is found
                }

                if (user == null || user.Password != password)
                {
                    await DisplayAlert("Login Failed", "Invalid username or password.", "OK");
                    return;
                }

                // Set the current user
                UserInfo.Current = user; // Set the current user after successful login

                // Save infos if "Remember Me" is checked
                if (rememberMeCheckBox.IsChecked)
                {
                    Preferences.Set("username", username);
                    await SecureStorage.SetAsync("password", password);
                }
                else
                {
                    // Clear saved infos if "Remember Me" is not checked
                    Preferences.Remove("username");
                    SecureStorage.Remove("password");
                }

                // Successful login
                if (Application.Current is App app)
                {
                    app.OnLoginSuccess(); // Navigate to AppShell
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
                Console.WriteLine($"Error during login: {ex.Message}");
            }
        }

        private void OnPasswordToggleTapped(object sender, EventArgs e)
        {
            // Password toggle
            entryPassword.IsPassword = !entryPassword.IsPassword;

            // Update the toggle
            passwordToggle.Text = entryPassword.IsPassword ? "Show" : "Hide";
        }

        private async void OnSignUpTapped(object sender, EventArgs e)
        {
            // Navigate to the Sign Up page
            await Navigation.PushAsync(new SignUpPage());
        }
    }
}