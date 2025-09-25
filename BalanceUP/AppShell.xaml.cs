using BalanceUP.Pages;
using Microsoft.Maui.Controls;

namespace BalanceUP
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Navigation Bar colors for the entire Shell
            Shell.SetBackgroundColor(this, Color.FromHex("#89CFF0"));

            // Register all routes for navigation
            Routing.RegisterRoute("login", typeof(LoginPage));
            Routing.RegisterRoute("home", typeof(HomePage));
            Routing.RegisterRoute("addexpense", typeof(AddExpensePage));
            Routing.RegisterRoute("chart", typeof(ChartPage));
            Routing.RegisterRoute("profile", typeof(ProfilePage));
            Routing.RegisterRoute("setbudget", typeof(SetBudgetPage));
        }
    }
}