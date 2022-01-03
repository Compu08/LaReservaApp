using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using TurnosFutbol.Models.Navigation;
using TurnosFutbol.Views.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;

namespace TurnosFutbol.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SendMessage : PopupPage
    {
        public FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");
        public ObservableCollection<string> types;
        public ObservableCollection<string> places;
        public string selectedPlaceImage;
        public SendMessage(bool bossPower)
        {
            InitializeComponent();
            CrossLabel.Text = "\uf00d";
            places = new ObservableCollection<string>();
            types = new ObservableCollection<string>();
            if (!bossPower)
            {
                TypePickerConstructor();
                adminDifusion.IsVisible = true;
                bossDifusion.IsVisible = false;
            }
            else {
                adminDifusion.IsVisible = false;
                bossDifusion.IsVisible = true;
            }
        }
        private void CloseModal(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }
        private void SendBossDifusion(object sender, EventArgs e) {
            PopupNavigation.Instance.PushAsync(new Loading());
            string destination = "";
            if (PublicPicker.SelectedItem.ToString() == "TODOS")
            {
                destination = "all";
            }
            else if (PublicPicker.SelectedItem.ToString() == "ADMNISTRADORES") {
                destination = "admins";
            }
            var fcmPush = new FCMNotification();
            fcmPush.SendNotification(BossEntryTitle.Text, BossEntryNotification.Text, "/topics/"+destination);
            PopupNavigation.Instance.PopAllAsync();
        }
        private void SendDifusion(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new Loading());
            var placeStr = (PlacePicker.SelectedItem.ToString()).Replace(" ", "_");
            var fcmPush = new FCMNotification();
            fcmPush.SendNotification(TitlePicker.SelectedItem+" - "+PlacePicker.SelectedItem, EntryNotification.Text,
               "/topics/"+placeStr);
            PushNotificationToDatabase();
            PopupNavigation.Instance.PopAllAsync();
        }

        private async void PushNotificationToDatabase()
        {
            Random rnd = new Random();
            int randomId = rnd.Next(1, 9999);
            await fc
                .Child("Notificaciones")
                .PostAsync(new Models.Navigation.Notification()
                {
                    Time = DateTime.Now,
                    Description = EntryNotification.Text,
                    Icon = CheckNotifIcon(),
                    User = Preferences.Get("user",""),
                    Token = Preferences.Get("token",""),
                    Topic = selectedPlaceImage,
                    Id = randomId, //RandomId
                    Phone = Preferences.Get("phone",""),
                    Title = TitlePicker.SelectedItem.ToString(),
                    Place = PlacePicker.SelectedItem.ToString()
                });
            
            await DisplayAlert("¡Exito!",
                "Se envió la Difusión de " + TitlePicker.SelectedItem.ToString() + " a nombre de " +
                PlacePicker.SelectedItem.ToString(), "Ok");

        }

        private async void GetPlaceImage()
        {
            var placesImage = (await fc
                .Child("Clients")
                //.Child(placeObject.Key)
                .OnceAsync<ClubsList>()).Select(item => new ClubsList()
            {
                place = item.Object.place,
                picture = item.Object.picture
            }).OrderBy(i => i.place);

            var placeObject2 = placesImage.First(a => a.place == PlacePicker.SelectedItem.ToString());

            selectedPlaceImage = placeObject2.picture;
        }

        private string CheckNotifIcon()
        {
            var icon = "";

            switch (TitlePicker.SelectedItem.ToString())
            {

                case "OFERTA":
                    icon = "\uf02c";
                    break;
                case "Difusión Turnos":
                    icon = "\uf017";
                    break;
                case "Información Torneo":
                    icon = "\uf091";
                    break;
                case "Nueva Noticia":
                    icon = "\uf1ea";
                    break;
            }

            return icon;
        }

        public async void TypePickerConstructor()
        {
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
            TypePicker.ItemsSource = types;
            TypePicker.SelectedIndex = 0;
            //LabelPickerType.Text = TypePicker.SelectedItem.ToString();
            PlacePickerConstructor(TypePicker.SelectedItem.ToString());
        }

        public async void PlacePickerConstructor(string type)
        {
            var typePickerDB = (await fc
                .Child("Admins")
                .Child(Preferences.Get("adminKey", ""))
                .Child(type.ToString())
                .OnceAsync<Admins>()).Select(item => new Admins
            {
                place = item.Object.place
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
            PlacePicker.ItemsSource = places;
            PlacePicker.SelectedIndex = 0;
            GetPlaceImage();
            //LabelPickerPlace.Text = PlacePicker.SelectedItem.ToString();
        }

        private void PlacePickerChanged(object sender, EventArgs e)
        {
            GetPlaceImage();
        }
    }
}