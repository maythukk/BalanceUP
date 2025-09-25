using Microsoft.Maui.Controls;
using System;
using BalanceUP.Data;
using BalanceUP.Models;
namespace BalanceUP.Pages
{
    public partial class SetBudgetPage : ContentPage
    {
        public SetBudgetPage()
        {
            InitializeComponent();
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                // Validate the input
                string amountText = entryAmount.Text;
                if (string.IsNullOrWhiteSpace(amountText) || !decimal.TryParse(amountText, out decimal amount))
                {
                    await DisplayAlert("Invalid Input", "Please enter a valid budget amount.", "OK");
                    return;
                }
                // Check if budget already exists for this month
                DatabaseHelper databaseHelper = new DatabaseHelper();
                var existingBudget = databaseHelper.GetBudgetForCurrentMonth();
                if (existingBudget != null)
                {
                    await DisplayAlert("Budget Already Set", "You can only set your budget once per month.", "OK");
                    entryAmount.Text = string.Empty; // Clear the field
                    return;
                }
                // Save the budget
                databaseHelper.SaveBudget(amount);
                // Show success message
                await DisplayAlert("Success", "Your budget has been set! Go to Homepage.", "OK");
                entryAmount.Text = string.Empty;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving budget: {ex.Message}");
                await DisplayAlert("Error", "Something went wrong. Please try again.", "OK");
            }
        }
    }
}