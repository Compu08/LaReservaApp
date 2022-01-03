using Firebase.Database;
using Firebase.Database.Query;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnosFutbol.Models.Navigation;
using TurnosFutbol.ViewModels.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TurnosFutbol.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddPlace : ContentPage
    {
        public ObservableCollection<OneSpot> spotList;
        public AddPlace()
        {
            InitializeComponent();
            PopupNavigation.Instance.PushAsync(new Popups.Loading());
            PlaceCity.BindingContext = new CityViewModel();
            PopupNavigation.Instance.PopAsync();
        }

        private string CapitalizeText(string str)
        {
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        private async void SaveClicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new Loading());
            var aux = 0;
            var sP = new GroupedSpots();
            while (aux != Convert.ToInt32(PlaceCourts.Text)) { 
                sP.Add(new OneSpot { price = Convert.ToInt32(PlaceShiftPrice.Text), number = aux+1, type = "" });
                aux++;
            }

            FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");
            Random rnd = new Random();
            int randomId = rnd.Next(1, 9999);
            await fc
            .Child("Clients")
            .PostAsync(new ClubsList()
            {
                place = CapitalizeText(PlaceName.Text),
                id = randomId,
                price = PlacePrice.Text,
                payment = false,
                hide = false,
                type = PlaceType.SelectedItem.ToString(),
                courts = PlaceCourts.Text,
                seller = Preferences.Get("user", "¡ERROR! NO USER ID"),
                adress = CapitalizeText(PlaceAdress.Text),
                phone = PlacePhone.Text,
                picture = "https://firebasestorage.googleapis.com/v0/b/lareserva-9d2f1.appspot.com/o/NoProfilePicture.png?alt=media&token=8218c680-291d-4b91-8d1f-66d29355a2cb",
                value = PlaceShiftPrice.Text,
                city = PlaceCity.SelectedItem.ToString(),
                vip = false,
                contacto = PlaceContact.Text,
                timebefore = TimeSpan.Zero,
                spotList = sP,
            }); 
            
            await DisplayAlert("¡Exito!", PlaceName.Text + " fué agregado correctamente.", "Ok");
            await PopupNavigation.Instance.PopAsync();
            await Navigation.PopModalAsync();

        }

        private async void GoBack(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}