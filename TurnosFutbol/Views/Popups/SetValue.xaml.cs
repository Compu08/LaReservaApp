using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using TurnosFutbol.Models.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TurnosFutbol.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetValue : PopupPage
    {
        private readonly FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");
        public ObservableCollection<string> types;
        public ObservableCollection<string> places;
        public string selectedPlaceImage;
        public int id;
        public string t;
        public string p;

        public SetValue(string place, int idPlace, string type)
        {
            InitializeComponent();
            CrossLabel.Text = "\uf00d";
            p = place;
            id = idPlace;
            t = type;
            places = new ObservableCollection<string>();
            types = new ObservableCollection<string>();
        }
        private void CloseModal(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }


        private async void PushValueToDatabase(object sender, EventArgs e)
        {
            var toUpdatePlace = (await fc
                .Child("Clients")
                .OnceAsync<ClubsList>()).ToList();

            var placeObject = toUpdatePlace.First(a => a.Object.place == p);

            await fc.Child("Clients").Child(placeObject.Key).PutAsync(new ClubsList()
            {
                adress = placeObject.Object.adress,
                city = placeObject.Object.city,
                contacto = placeObject.Object.contacto,
                courts = placeObject.Object.courts,
                hide = placeObject.Object.hide,
                hideC = placeObject.Object.hideC,
                id = placeObject.Object.id,
                payment = placeObject.Object.payment,
                paymentC = placeObject.Object.paymentC,
                phone = placeObject.Object.phone,
                picture = placeObject.Object.picture,
                place = placeObject.Object.place,
                seller = placeObject.Object.seller,
                timebefore = placeObject.Object.timebefore,
                type = placeObject.Object.type,
                value = EntryNewPrice.Text,
                vip = placeObject.Object.vip,
                spotList = placeObject.Object.spotList
            });
            await DisplayAlert("¡Exito!",
                "Se asignó el valor $" + EntryNewPrice.Text + " a " +
                p, "Ok");

            await PopupNavigation.Instance.PopAllAsync();

        }

    }
}