using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public partial class TimeBefore : PopupPage
    {
        public FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");
        public ObservableCollection<string> types;
        public ObservableCollection<string> places;

        public TimeBefore()
        {
            InitializeComponent();
            CrossLabel.Text = "\uf00d";
            places = new ObservableCollection<string>();
            types = new ObservableCollection<string>();
            TypePickerConstructor();

        }
        private void CloseModal(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }

        public async void TypePickerConstructor()
        {
            IOrderedEnumerable<Admins> typePickerDB = (await fc
                .Child("Admins")
                .Child(Preferences.Get("adminKey", ""))
                .OnceAsync<Admins>()).Select(item => new Admins
            {
                place = item.Key
            }).ToList().OrderBy(i => i.place);

            if (typePickerDB.Count() != 0)
            {
                foreach (Admins item in typePickerDB)
                {
                    Admins placeName = new Admins() { place = item.place };
                    types.Add(placeName.place.ToString());
                }
            }
            typePicker.ItemsSource = types;
            typePicker.SelectedIndex = 0;
            PlacePickerConstructor(typePicker.SelectedItem.ToString());
        }
        public async void PlacePickerConstructor(string type)
        {
            var typePickerDB = (await fc
                .Child("Admins")
                .Child(Preferences.Get("adminKey", ""))
                .Child(type.ToString())
                .OnceAsync<Admins>()).Select(item => new Admins
            {
                place = item.Object.place,
                }).ToList().OrderBy(i => i.place);

            if (typePickerDB.Count() != 0)
            {
                places.Clear();
                foreach (Admins item in typePickerDB)
                {
                    Admins placeName = new Admins() { place = item.place };
                    places.Add(placeName.place.ToString());
                }
            }
            placePicker.ItemsSource = places;
            placePicker.SelectedIndex = 0;
        }

        private void TypePicked(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new Popups.Loading());
            PlacePickerConstructor(typePicker.SelectedItem.ToString());
            PopupNavigation.Instance.PopAsync();
        }

        private void PlacePicked(object sender, EventArgs e)
        {
            if (typePicker.SelectedIndex != -1 && placePicker.SelectedIndex != -1)
            {
                LoadTimeOfPlace();
            }
        }

        private async void LoadTimeOfPlace()
        {
          //  await PopupNavigation.Instance.PushAsync(new Loading());
            var itemPicked = (await fc
                .Child("Clients")
                .OnceAsync<Models.Navigation.ClubsList>()).ToList();

            var placeObject = itemPicked.First(a => a.Object.place == placePicker.SelectedItem.ToString());

            PickerTime.Time = placeObject.Object.timebefore;
            // await PopupNavigation.Instance.PopAsync();
        }

        private void Type_Tapped(object sender, EventArgs e)
        {
            typePicker.Focus();
        }

        private void Place_Tapped(object sender, EventArgs e)
        {
            placePicker.Focus();
        }
        private void SaveTimeBefore(object sender, EventArgs e)
        {
            var theTime = PickerTime.Time;
            SaveTimeInDatabase(theTime);
        }

        private async void SaveTimeInDatabase(TimeSpan time)
        {
            await PopupNavigation.Instance.PushAsync(new Loading());

                var toUpdatePlaceList = (await fc
                    .Child("Clients")
                    .OnceAsync<Models.Navigation.ClubsList>()).ToList();

                var placeObject = toUpdatePlaceList.First(a => a.Object.place == placePicker.SelectedItem.ToString());

                await fc.Child("Clients").Child(placeObject.Key).PutAsync(new ClubsList()
                {
                    id = placeObject.Object.id,
                    place = placeObject.Object.place,
                    price = placeObject.Object.price,
                    payment = placeObject.Object.payment,
                    hide = placeObject.Object.hide,
                    hideC = placeObject.Object.hideC,
                    paymentC = placeObject.Object.paymentC,
                    type = placeObject.Object.type,
                    seller = placeObject.Object.seller,
                    adress = placeObject.Object.adress,
                    city = placeObject.Object.city,
                    contacto = placeObject.Object.contacto,
                    courts = placeObject.Object.courts,
                    phone = placeObject.Object.phone,
                    picture = placeObject.Object.picture,
                    value = placeObject.Object.value,
                    vip = placeObject.Object.vip,
                    timebefore = time,
                    spotList = placeObject.Object.spotList,
                });

                await DisplayAlert("¡Exito!","Se modificó el tiempo de reserva con anterioridad para " + placePicker.SelectedItem.ToString(),
                    "Ok");

                await PopupNavigation.Instance.PopAllAsync();
        }
    }
}