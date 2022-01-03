using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using Rg.Plugins.Popup.Services;
using TurnosFutbol.Models.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TurnosFutbol.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ManageSpots : ContentPage
    {
        private readonly FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");
        public ObservableCollection<string> types;
        public ObservableCollection<string> places;
        public ObservableCollection<string> col;
        public GroupedSpots spotL;
        public ObservableCollection<OneSpot> lstSource;
        public string TextType;

        public ManageSpots()
        {
            InitializeComponent();
            lstView.SelectionBackgroundColor = (Color)Application.Current.Resources["ButtonBackground"];
            places = new ObservableCollection<string>();
            types = new ObservableCollection<string>();
            lstSource = new ObservableCollection<OneSpot>();
            col = new ObservableCollection<string>();
            spotL = new GroupedSpots();
            PopupNavigation.Instance.PushAsync(new Loading());
            TypePickerConstructor();
            PopupNavigation.Instance.PopAsync();
            //CreateAndFillLabels();
        }

        private async void CreateAndFillLabels()
        {
            await PopupNavigation.Instance.PushAsync(new Loading());
            var getClubsInfo = (await fc
            .Child("Clients")
            .OnceAsync<ClubsList>()).Select(item => new ClubsList
            {
                place = item.Object.place,
                picture = item.Object.picture,
                adress = item.Object.adress,
                value = "Turno: $" + item.Object.value,
                courts = item.Object.courts,
                city = item.Object.city,
                phone = item.Object.phone,
                hide = item.Object.hide,
                type = item.Object.type,
                vip = item.Object.vip,
                timebefore = item.Object.timebefore,
                spotList = item.Object.spotList,
            }).Where(a => a.place == PlacePicker.SelectedItem.ToString()).ToList().OrderBy(i => i.place);

            TypeInitializer();
            lstSource.Clear();
            foreach (ClubsList item in getClubsInfo)
            {
                foreach (var c in item.spotList)
                {
                    lstSource.Add(new OneSpot { spotString = TextType + " " + c.number, type = c.type, priceString = "$" + c.price, price = c.price, number = c.number});
                }
            }
            lstView.ItemsSource = lstSource;

            await PopupNavigation.Instance.PopAllAsync();
        }

        private void PushSpotEditor(object sender, Syncfusion.ListView.XForms.ItemTappedEventArgs e)
        {
            var selectedObj = e.ItemData as OneSpot;
            var popupPage = new SpotEditor(selectedObj.number, selectedObj.type, selectedObj.price, TextType, col, TypePicker.SelectedItem.ToString(),PlacePicker.SelectedItem.ToString());

            popupPage.Disappearing += ReloadList; 
            PopupNavigation.Instance.PushAsync(popupPage);
        }

        private void ReloadList(object sender, EventArgs e)
        {
            CreateAndFillLabels();
        }

        private void TypeInitializer()
        {
            col.Clear();
            switch (TypePicker.SelectedItem.ToString())
            {
                case "Futbol":
                    col.Add("Futbol 5");
                    col.Add("Futbol 7");
                    col.Add("Futbol 11");
                    TextType = "CANCHA";
                    break;
                case "Tenis":
                    col.Add("Polvo de Ladrillo");
                    col.Add("Cesped");
                    col.Add("Cemento");
                    TextType = "CANCHA";
                    break;
                case "Paddel":
                    col.Add("Cemento");
                    col.Add("Cesped");
                    TextType = "CANCHA";
                    break;
                case "Peluqueria":
                    TextType = "SILLA";
                    break;
                case "Restaurantes":
                    TextType = "MESA";
                    break;
                case "Gimnasios":
                    TextType = "CLASES";
                    break;
            }

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
            PlacePickerConstructor(TypePicker.SelectedItem.ToString());
            TypePickerLabel.Text = TypePicker.SelectedItem.ToString();
        }

        public async void PlacePickerConstructor(string type)
        {
            IOrderedEnumerable<Admins> typePickerDB = (await fc
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
        }

        private void PlacePickerChanged(object sender, EventArgs e)
        {
            if (TypePicker.SelectedIndex != -1 && PlacePicker.SelectedIndex != -1)
            {
                PlacePickerLabel.Text = PlacePicker.SelectedItem.ToString();
                CreateAndFillLabels();
            }
        }
        private void TypePickerChanged(object sender, EventArgs e)
        {
            TypePickerLabel.Text = TypePicker.SelectedItem.ToString();
            if (TypePicker.SelectedItem.ToString() == "Gimnasios") {
                lstView.IsVisible = false;
                ButtonEditValue.IsVisible = true;
            }
            PopupNavigation.Instance.PushAsync(new Loading());
            PlacePickerConstructor(TypePicker.SelectedItem.ToString());
            PopupNavigation.Instance.PopAsync();
        }

        private void SaveChanges(object sender, EventArgs e)
        {

        }

        private void GoBack(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private void PlaceTapped(object sender, EventArgs e)
        {
            PlacePicker.Focus();
        }
        private void TypeTapped(object sender, EventArgs e)
        {
            TypePicker.Focus();
        }

        private void PushSetValuePopup(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new SetValue(PlacePicker.SelectedItem.ToString(), 0, TypePicker.SelectedItem.ToString()));
        }
    }
}