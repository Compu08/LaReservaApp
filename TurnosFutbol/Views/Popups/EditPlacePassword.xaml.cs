using Firebase.Database;
using Firebase.Database.Query;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnosFutbol.Models.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TurnosFutbol.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditPlacePassword : PopupPage
    {
        public FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");
        public ObservableCollection<string> types;
        public ObservableCollection<string> places;

        public EditPlacePassword()
        {
            InitializeComponent();
            CrossLabel.Text = "\uf00d";
            places = new ObservableCollection<string>();
            types = new ObservableCollection<string>();
            TypePickerConstructor();
        }

        private void CloseModal(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAllAsync();
        }

        public async void TypePickerConstructor()
        {
            await PopupNavigation.Instance.PushAsync(new Loading());
            var typePickerDB = (await fc
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
            await PopupNavigation.Instance.PopAsync();
        }
        public async void PlacePickerConstructor(string type)
        {
            await PopupNavigation.Instance.PushAsync(new Loading());
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
            await PopupNavigation.Instance.PopAsync();
        }

        private void TypePicked(object sender, EventArgs e)
        {
            PlacePickerConstructor(typePicker.SelectedItem.ToString());
        }

        private void PlacePicked(object sender, EventArgs e)
        {
            if (typePicker.SelectedIndex != -1 && placePicker.SelectedIndex != -1)
            {
                GetData();
            }
        }

        private async void GetData() {
            
            var theData = (await fc
                    .Child("Clients")
                    .OnceAsync<ClubsList>()).Select(item => new ClubsList
                    {
                        place = item.Object.place,
                        password = item.Object.password,
                        isLocked = item.Object.isLocked,
                    }).Where(a=>a.place == placePicker.SelectedItem.ToString()).FirstOrDefault();

            isOn.IsChecked = theData.isLocked;
            thePassword.Text = theData.password;
        }

        private async void SavePassword(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new Loading());

            var toUpdatePlaceList = (await fc
                .Child("Clients")
                .OnceAsync<ClubsList>()).ToList();

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
                timebefore = placeObject.Object.timebefore,
                spotList = placeObject.Object.spotList,
                password = thePassword.Text,
                isLocked = isOn.IsChecked
            });

            await DisplayAlert("¡Exito!", "Contraseña modificada exitosamente", "Ok");

            await PopupNavigation.Instance.PopAllAsync();
        }
    }
}