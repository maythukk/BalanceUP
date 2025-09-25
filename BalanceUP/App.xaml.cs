using BalanceUP.Pages;
using Microsoft.Maui.Controls;

namespace BalanceUP
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Start with LoadingPage
            return new Window(new NavigationPage(new LoadingPage()));
        }

        // Navigate to AppShell after login
        public void OnLoginSuccess()
        {
            if (Windows.Count > 0)
            {
                Console.WriteLine("Login successful, navigating to AppShell.");
                Windows[0].Page = new AppShell(); // Switch to AppShell after login
            }
        }
    }
}