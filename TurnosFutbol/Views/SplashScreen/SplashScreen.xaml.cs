using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TurnosFutbol.Views.SplashScreen
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SplashScreen : ContentPage
    {
        bool registered = Preferences.Get("registered", false);
        public SplashScreen()
        {
            InitializeComponent();
            TimerCloser();
        }
        private async void TimerCloser()
        {
            await Task.Delay(3000);
            if (registered)
            {
                await Navigation.PushAsync(new Navigation.BottomNavigationPage());
            }
            else
            {
                await Navigation.PushAsync(new Forms.LoginPage());
            }
        }
    }
}