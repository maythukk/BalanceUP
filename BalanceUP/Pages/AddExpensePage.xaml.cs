using Microsoft.Maui.Controls;
using System;
using BalanceUP.Data;
using BalanceUP.Models;
namespace BalanceUP.Pages
{
    public partial class AddExpensePage : ContentPage
    {
        public AddExpensePage()
        {
            InitializeComponent();
            // Set the default date to today
            datePicker.Date = DateTime.Today;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                // Get the amount
                string amountText = entryAmount.Text;
                string selectedCategory = pickerCategory.SelectedItem?.ToString();
                DateTime selectedDate = datePicker.Date;

                // Validate amount
                if (string.IsNullOrWhiteSpace(amountText) || !decimal.TryParse(amountText, out decimal amount))
                {
                    await DisplayAlert("Invalid Input", "Please enter a valid amount.", "OK");
                    return;
                }

                // Validate category
                if (string.IsNullOrWhiteSpace(selectedCategory))
                {
                    await DisplayAlert("Missing Category", "Please select a category for your expense.", "OK");
                    return;
                }

                // Create a new Expense
                Expense expense = new Expense
                {
                    Amount = amount,
                    Category = selectedCategory,
                    Date = selectedDate // Use selected date from DatePicker
                };

                // Save the expense to the database
                DatabaseHelper databaseHelper = new DatabaseHelper();
                databaseHelper.InsertExpense(expense);

                // success message
                await DisplayAlert("Success", "Expense added successfully!", "OK");

                // Clear the input fields
                OnClearClicked(sender, e);
            }
            catch (Exception ex)
            {
                // exception message
                Console.WriteLine($"Error: {ex.Message}");
                await DisplayAlert("Error", "An error occurred while saving the expense. Please try again.", "OK");
            }
        }

        private void OnClearClicked(object sender, EventArgs e)
        {
            // Clear inputs
            entryAmount.Text = string.Empty;
            pickerCategory.SelectedItem = null;
            datePicker.Date = DateTime.Today; // Reset date to today
        }
    }
}