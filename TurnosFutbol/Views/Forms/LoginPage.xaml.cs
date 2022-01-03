using Plugin.FirebasePushNotification;
using System.Globalization;
using Xamarin.Essentials;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace TurnosFutbol.Views.Forms
{
    /// <summary>
    /// Page to login with user name and password
    /// </summary>
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginPage" /> class.
        /// </summary>
        public LoginPage()
        {
            InitializeComponent();
            CrossFirebasePushNotification.Current.UnsubscribeAll();
        }

        private async void SaveAndGoNextPage(object sender, System.EventArgs e)
        {
            if (CheckButtons())
            {
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                var strUser = textInfo.ToTitleCase(UserEntry.Text.ToLower());
                Preferences.Set("user", strUser);
                Preferences.Set("phone", GetPhone());
                Preferences.Set("registered", true);
                Navigation.InsertPageBefore(new Navigation.BottomNavigationPage(), this);
                await Navigation.PopAsync();
                //await Navigation.PushAsync(new Navigation.BottomNavigationPage());
            }
            else
            {
                await DisplayAlert("¡Error!", "El campo Nombre y Apellido es obligatorio", "Ok");
            }
        }

        private bool CheckButtons()
        {
            return !string.IsNullOrEmpty(UserEntry.Text);
        }

        private string GetPhone()
        {
            return PasswordEntry.Text != "" ? PasswordEntry.Text : "";
        }
    }
}