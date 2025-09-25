using Microsoft.Maui.Controls;
using System;
using System.Text.RegularExpressions;
using BalanceUP.Data;
using BalanceUP.Models;

namespace BalanceUP.Pages
{
    public partial class ChangePasswordPage : ContentPage
    {
        public ChangePasswordPage()
        {
            InitializeComponent();
        }

        private async void OnChangePasswordClicked(object sender, EventArgs e)
        {
            string currentPassword = CurrentPasswordEntry.Text;
            string newPassword = NewPasswordEntry.Text;
            string confirmPassword = ConfirmPasswordEntry.Text;

            // Validate current password
            var currentUser = UserInfo.Current;
            if (currentUser != null && currentUser.Password == currentPassword)
            {
                // Validate new password
                if (!IsValidPassword(newPassword))
                {
                    await DisplayAlert("Error", "New password does not meet the requirements.", "OK");
                    return;
                }

                // Check if new password matches confirmation
                if (newPassword != confirmPassword)
                {
                    await DisplayAlert("Error", "New password and confirmation do not match.", "OK");
                    return;
                }

                // Update the password
                currentUser.Password = newPassword;
                DatabaseHelper databaseHelper = new DatabaseHelper();
                databaseHelper.UpdateUser(currentUser);

                await DisplayAlert("Success", "Your password has been changed successfully.", "OK");
                await Navigation.PopAsync(); // Go back to the previous page
            }
            else
            {
                await DisplayAlert("Error", "Current password is incorrect.", "OK");
            }
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