using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BalanceUP.Data;
using BalanceUP.Models;
using Microsoft.Maui.Graphics;

namespace BalanceUP.Pages
{
    public partial class HomePage : ContentPage
    {
        private SwipeView _activeSwipeView;
        private decimal currentBudget = 0;
        private decimal currentExpenses = 0;

        public HomePage()
        {
            InitializeComponent();
            BindingContext = this;
            LoadBudget();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            RefreshPageData();
        }

        public void RefreshPageData()
        {
            LoadExpenses();
            LoadTotalExpenses();
            LoadBudget();
            UpdateBudgetProgress();
        }

        private void LoadBudget()
        {
            try
            {
                DatabaseHelper databaseHelper = new DatabaseHelper();
                var budget = databaseHelper.GetBudgetForCurrentMonth();

                if (budget != null)
                {
                    currentBudget = budget.Amount;
                    lblBudgetAmount.Text = "$" + budget.Amount.ToString("F0");
                }
                else
                {
                    currentBudget = 0;
                    lblBudgetAmount.Text = "$0";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading budget: {ex.Message}");
                currentBudget = 0;
                lblBudgetAmount.Text = "$0";
            }
        }

        private void LoadExpenses()
        {
            try
            {
                DatabaseHelper databaseHelper = new DatabaseHelper();
                List<Expense> expenses = databaseHelper.GetAllExpenses();

                Console.WriteLine($"LoadExpenses: Retrieved {expenses.Count} expenses from database");

                if (expenses.Count == 0)
                {
                    // No expenses found
                    listViewTodaySpendings.ItemsSource = null;
                    lblNoSpendingsToday.IsVisible = true;
                    return;
                }

                // Group expenses by date
                var groupedExpenses = expenses
                    .GroupBy(e => e.Date.Date)
                    .OrderByDescending(g => g.Key)
                    .Select(g => new ExpenseGroup(g.Key, g.ToList()))
                    .ToList();

                // Set the grouped expenses as the item source for the CollectionView
                listViewTodaySpendings.ItemsSource = groupedExpenses;

                // Debug the groups
                Console.WriteLine($"Created {groupedExpenses.Count} expense groups");
                foreach (var group in groupedExpenses)
                {
                    Console.WriteLine($"Group: {group.Date.ToShortDateString()} has {group.Items.Count} expenses");
                }

                // Hide the "No spendings" label
                lblNoSpendingsToday.IsVisible = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading expenses: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                DisplayAlert("Error", $"Failed to load expenses: {ex.Message}", "OK");
            }
        }

        private void LoadTotalExpenses()
        {
            DatabaseHelper databaseHelper = new DatabaseHelper();
            currentExpenses = databaseHelper.GetTotalExpenses();
            lblExpensesAmount.Text = "$" + currentExpenses.ToString("F0");
        }

        private void UpdateBudgetProgress()
        {
            if (currentBudget <= 0)
            {
                // Show empty bar if no budget is set
                budgetProgressBar.WidthRequest = 0;
                budgetProgressBar.BackgroundColor = Colors.Transparent;
                lblBudgetStatus.Text = "Set a budget";
                return;
            }

            // Check if there are no expenses
            if (currentExpenses == 0)
            {
                // No expenses >> set progress bar to 0 and transparent
                budgetProgressBar.WidthRequest = 0;
                budgetProgressBar.BackgroundColor = Colors.Transparent;
                lblBudgetStatus.Text = "No expenses yet";
                lblBudgetStatus.TextColor = Colors.Gray;
                return;
            }

            // Calculate percentage of budget used
            double percentage = (double)(currentExpenses / currentBudget * 100);

            // Cap at 100% for UI purposes (will still show red over 100%)
            double barWidth = Math.Min(percentage, 100);

            // Get the parent Frame's width to calculate actual width
            double parentWidth = ((Frame)budgetProgressBar.Parent).Width;
            budgetProgressBar.WidthRequest = parentWidth * (barWidth / 100);

            // Update colors based on percentage
            if (percentage >= 100)
            {
                // Over budget - Red
                budgetProgressBar.BackgroundColor = Colors.Red;
                lblBudgetStatus.Text = "Over budget!";
                lblBudgetStatus.TextColor = Colors.Red;
            }
            else if (percentage >= 80)
            {
                // Near budget - Yellow
                budgetProgressBar.BackgroundColor = Color.FromArgb("#FFC107");
                lblBudgetStatus.Text = $"{percentage:F0}% used";
                lblBudgetStatus.TextColor = Color.FromArgb("#FFC107");
            }
            else
            {
                // Within budget - Green
                budgetProgressBar.BackgroundColor = Color.FromArgb("#4CAF50");
                lblBudgetStatus.Text = $"{percentage:F0}% used";
                lblBudgetStatus.TextColor = Color.FromArgb("#4CAF50");
            }
        }


        // Event handler for when SwipeView opens
        private void OnSwipeStarted(object sender, SwipeStartedEventArgs e)
        {
            _activeSwipeView = sender as SwipeView;
        }

        // Event handler for the swipe to delete action
        private async void OnDeleteSwipeItemInvoked(object sender, EventArgs e)
        {
            var swipeItem = sender as SwipeItem;
            var expense = swipeItem?.BindingContext as Expense;

            if (expense != null)
            {
                // confirmation
                bool confirm = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this expense?", "Yes", "No");

                if (confirm)
                {
                    DatabaseHelper databaseHelper = new DatabaseHelper();
                    databaseHelper.DeleteExpense(expense.Id);
                    RefreshPageData();
                }

                // Close the swipe view as the user's choice
                if (_activeSwipeView != null)
                {
                    _activeSwipeView.Close();
                    _activeSwipeView = null;
                }
            }
        }

        // Handle size changed event to adjust progress bar after layout
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            // Update progress bar size when layout changes
            if (width > 0 && height > 0)
            {
                // Delay slightly to ensure parent layout is complete
                Dispatcher.Dispatch(() => UpdateBudgetProgress());
            }
        }
    }

    // Modified ExpenseGroup class
    public class ExpenseGroup : List<Expense>
    {
        public DateTime Date { get; private set; }
        public List<Expense> Items => this;

        public ExpenseGroup(DateTime date, List<Expense> expenses) : base(expenses)
        {
            Date = date;
        }
    }
}