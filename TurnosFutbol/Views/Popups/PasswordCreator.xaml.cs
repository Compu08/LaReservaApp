using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using Rg.Plugins.Popup.Services;
using Syncfusion.XForms.Buttons;
using TurnosFutbol.Models.Navigation;
using TurnosFutbol.ViewModels.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TurnosFutbol.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PasswordCreator : ContentPage
    {
        private static readonly Random Random = new Random();
        private readonly ObservableCollection<string> placesPicked;
        private readonly Dictionary<string, List<string>> typesPicked;
        private readonly FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");

        private ObservableCollection<ClubsList> placesObject;

        private ObservableCollection<ClubsList> PlacesObject
        {
            get => placesObject;
            set
            {
                placesObject = value;
                OnPropertyChanged();
            }
        }


        public PasswordCreator()
        {
            InitializeComponent();
            PopupNavigation.Instance.PushAsync(new Popups.Loading());
            placesPicked = new ObservableCollection<string>();
            typesPicked = new Dictionary<string, List<string>>();
            CityPicker.BindingContext = new CityViewModel();
            PopupNavigation.Instance.PopAsync();
        }

        private void GoBack(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private void GenerateRandomPassword(object sender, EventArgs e)
        {
            var password = GenerateRandomString(6);
            passwordLabel.Text = password;
            UploadPassword();
        }

        private static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        private async void FillPlaces(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new Popups.Loading());

            PlacesObject = new ObservableCollection<ClubsList>();

            var allPlaces = (await fc
                .Child("Clients")
                .OnceAsync<ClubsList>()).ToList();

            var placesList = allPlaces.Where(i => i.Object.city == CityPicker.SelectedItem.ToString());

            foreach (var item in placesList)
            {
                PlacesObject.Add(item.Object);
            }

            lstview.ItemsSource = PlacesObject;
            await PopupNavigation.Instance.PopAsync();
        }

        private void AddItemToCheckBoxList(object sender, StateChangedEventArgs e)
        {
            var type = (sender as SfCheckBox)?.ClassId;
            var obj = (sender as SfCheckBox)?.Text;

            if (e.IsChecked == true)
            {
                if (!typesPicked.ContainsKey(type)) typesPicked.Add(type, new List<string>());
                typesPicked[type].Add(obj);
            }else
            {
                typesPicked[type].Remove(obj);
                //placesPicked.Remove(obj);
            }

        }

        private async void UploadPassword()
        {
            foreach (var item in typesPicked)
            {
                foreach (var val in item.Value)
                {
                    await fc
                        .Child("Admins")
                        .Child(passwordLabel.Text)
                        .Child(item.Key)
                        .PostAsync(new PlacesForPass()
                        {
                            place = val
                        });
                }
            }

            await DisplayAlert("¡Exito!", "Se agregaron correctamente los lugares asignados para la contraseña "+passwordLabel.Text, "ok");

            await Navigation.PopModalAsync();
        }

    }
}