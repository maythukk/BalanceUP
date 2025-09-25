using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace BalanceUP.Pages
{
    public partial class LoadingPage : ContentPage
    {
        public LoadingPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Loading effect
            for (int i = 0; i < 3; i++)
            {
                LoadingLabel.Text = "Loading" + new string('.', i + 1);
                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            // Navigate to the LoginPage after loading
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}