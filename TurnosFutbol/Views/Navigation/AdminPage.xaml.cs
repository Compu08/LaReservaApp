using System;
using Rg.Plugins.Popup.Services;
using TurnosFutbol.Views.Popups;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TurnosFutbol.Views.Navigation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdminPage : ContentPage
    {
        public string nameTable;
        public string adminKey;
        public AdminPage()
        {
            InitializeComponent();
            adminKey = Preferences.Get("adminKey", "");
        }

        private void GoBack(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
        private async void Button_Clicked(object sender, EventArgs e)
        {

            // nameTable = labelNameTable.Text.ToString();
            // FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");
            // var result = await fc
            //    .Child(nameTable)
            //    .PostAsync(new ClubsList() { place = this.club.Text.ToString(), adress = this.adress.Text.ToString(), city = this.city.Text.ToString(), picture = this.picture.Text.ToString(), courts = this.courts.Text.ToString(), value = this.value.Text.ToString(), phone = this.phone.Text.ToString() });
        }

        public void ShiftsEditClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ShiftsEditor());
        }

        public void PasswordClicked(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new EditPlacePassword());
        }

        public void PopupDifusion(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new SendMessage(false));
        }

        public void ValueClicked(object sender, EventArgs e)
        {
            //PopupNavigation.Instance.PushAsync(new SetValue());
            Navigation.PushModalAsync(new ManageSpots());
        }

        private void TimeBeforeClicked(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new TimeBefore());
        }
    }
}