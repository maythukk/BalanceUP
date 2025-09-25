using Microsoft.Maui.Controls;
using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BalanceUP.Data;
using BalanceUP.Models;

namespace BalanceUP.Pages
{
    public partial class ChartPage : ContentPage
    {
        private DateTime currentDate;
        private DatabaseHelper _databaseHelper;

        private bool _eventsSubscribed = false;

        public ChartPage()
        {
            InitializeComponent();
            currentDate = DateTime.Now; // Initialize to the current date
            _databaseHelper = new DatabaseHelper(); // Initialize DatabaseHelper

            // Subscribe to page lifecycle events
            SubscribeToEvents();

            UpdateUI(); // Update the UI to show the current month and year
        }

        private void SubscribeToEvents()
        {
            if (!_eventsSubscribed)
            {
                // Subscribe to appearing event to refresh data
                this.Appearing += OnPageAppearing;
                _eventsSubscribed = true;
            }
        }

        private void OnPageAppearing(object sender, EventArgs e)
        {
            // Refresh the data when the page was clicked
            UpdateUI();
        }

        private void OnPreviousMonthClicked(object sender, EventArgs e)
        {
            // Check if the previous month has expenses
            DateTime previousMonth = currentDate.AddMonths(-1);
            if (HasExpensesForMonth(previousMonth))
            {
                currentDate = previousMonth;
                UpdateUI();
            }
            else
            {
                DisplayAlert("No Data", "There are no expenses recorded for the previous month.", "OK");
            }
        }

        private void OnNextMonthClicked(object sender, EventArgs e)
        {
            // Check if the next month has expenses
            DateTime nextMonth = currentDate.AddMonths(1);

            // Don't allow navigation to future months beyond current month
            if (nextMonth > DateTime.Now)
            {
                DisplayAlert("Not Available", "Cannot navigate to future months.", "OK");
                return;
            }

            if (HasExpensesForMonth(nextMonth))
            {
                currentDate = nextMonth;
                UpdateUI();
            }
            else
            {
                DisplayAlert("No Data", "There are no expenses recorded for the next month.", "OK");
            }
        }

        private bool HasExpensesForMonth(DateTime date)
        {
            try
            {
                // Get expenses for the specified month
                var expenses = _databaseHelper.GetExpensesByMonth(date);

                // Return true if there are any expenses
                return expenses != null && expenses.Any();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking expenses for month: {ex.Message}");
                return false;
            }
        }

        private void UpdateUI()
        {
            lblCurrentMonth.Text = currentDate.ToString("MMMM yyyy");

            // Update button states
            UpdateNavigationButtons();

            // Update chart and totals
            UpdatePieChart(currentDate);
            UpdateTotalSpendings();
        }

        private void UpdateNavigationButtons()
        {
            // Check availability of previous month
            DateTime previousMonth = currentDate.AddMonths(-1);
            btnPrevious.IsEnabled = HasExpensesForMonth(previousMonth);
            btnPrevious.Opacity = btnPrevious.IsEnabled ? 1.0 : 0.5;

            // Check availability of next month
            DateTime nextMonth = currentDate.AddMonths(1);
            bool nextMonthAvailable = nextMonth <= DateTime.Now && HasExpensesForMonth(nextMonth);
            btnNext.IsEnabled = nextMonthAvailable;
            btnNext.Opacity = btnNext.IsEnabled ? 1.0 : 0.5;
        }

        private void UpdatePieChart(DateTime date)
        {
            try
            {
                // Fetch expenses for the selected month
                var expenses = _databaseHelper.GetExpensesByMonth(date);
                var groupedExpenses = expenses.GroupBy(e => e.Category)
                                               .Select(g => new
                                               {
                                                   Category = g.Key,
                                                   TotalAmount = g.Sum(e => e.Amount)
                                               }).ToList();

                var entries = new List<ChartEntry>();

                // Clear previous category details
                categoryDetailsLayout.Children.Clear();

                // If no expenses for current month, show message
                if (!groupedExpenses.Any())
                {
                    var noDataLabel = new Label
                    {
                        Text = "No expenses recorded for this month.",
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        FontSize = 16
                    };
                    categoryDetailsLayout.Children.Add(noDataLabel);

                    // Set empty chart
                    chartView.Chart = new PieChart { Entries = entries };
                    return;
                }

                foreach (var expense in groupedExpenses)
                {
                    var color = GetColorForCategory(expense.Category);

                    // Add entry to chart
                    entries.Add(new ChartEntry((float)expense.TotalAmount)
                    {
                        Label = expense.Category,
                        ValueLabel = $"${expense.TotalAmount}",
                        Color = SKColor.Parse(color),
                        TextColor = SKColor.Parse("#000000"),
                        ValueLabelColor = SKColor.Parse("#000000")
                    });

                    // Create category detail
                    var categoryFrame = new Frame
                    {
                        Padding = new Thickness(10),
                        CornerRadius = 5,
                        HasShadow = false,
                        BorderColor = Color.FromArgb("#EEEEEE")
                    };

                    var categoryLayout = new Grid();
                    categoryLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
                    categoryLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                    categoryLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                    // Color indicator
                    var colorIndicator = new BoxView
                    {
                        Color = Color.FromHex(color),
                        WidthRequest = 20,
                        HeightRequest = 20,
                        VerticalOptions = LayoutOptions.Center
                    };

                    // Category name
                    var categoryLabel = new Label
                    {
                        Text = expense.Category,
                        FontAttributes = FontAttributes.Bold,
                        VerticalOptions = LayoutOptions.Center
                    };

                    // Amount
                    var amountLabel = new Label
                    {
                        Text = $"${expense.TotalAmount:0.00}",
                        HorizontalOptions = LayoutOptions.End,
                        VerticalOptions = LayoutOptions.Center
                    };

                    // Add elements to grid
                    categoryLayout.Add(colorIndicator, 0, 0);
                    categoryLayout.Add(categoryLabel, 1, 0);
                    categoryLayout.Add(amountLabel, 2, 0);

                    categoryFrame.Content = categoryLayout;
                    categoryDetailsLayout.Children.Add(categoryFrame);
                }

                // Update the chart
                var chart = new PieChart
                {
                    Entries = entries,
                    LabelMode = LabelMode.None // Hide labels
                };

                chartView.Chart = chart;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in UpdatePieChart: {ex.Message}");
                DisplayAlert("Error", $"Failed to update pie chart: {ex.Message}", "OK");
            }
        }

        private void UpdateTotalSpendings()
        {
            try
            {
                // Get total expenses for the selected month
                decimal totalSpendings = _databaseHelper.GetExpensesByMonth(currentDate)
                    .Sum(e => e.Amount);

                lblTotalSpendings.Text = $"Total Spendings: ${totalSpendings:0.00}";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in UpdateTotalSpendings: {ex.Message}");
                DisplayAlert("Error", $"Failed to update total spendings: {ex.Message}", "OK");
            }
        }

        private string GetColorForCategory(string category)
        {
            // Colors for each categories
            return category switch
            {
                "Food" => "#fbc9c3",
                "Shopping" => "#f0f4c3",
                "Bills" => "#aed6f1",
                "Entertainment" => "#e3f2fd",
                "Transportation" => "#ffebee",
                "Others" => "#fffde7",
            };
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
    }
}