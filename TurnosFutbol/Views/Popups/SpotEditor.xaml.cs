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
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json.Linq;


namespace TurnosFutbol.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SpotEditor : PopupPage
    {
        public FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");
        public int n;
        public string t;
        public int p;
        public string txt;
        public string typeOf;
        public string place;
        public SpotEditor(int number, string type, int price, string text, ObservableCollection<string> pickerCol, string theType, string theSpot)
        {
            InitializeComponent();
            CrossLabel.Text = "\uf00d";
            n = number;
            t = type;
            p = price;
            txt = text;
            labelSpot.Text = text + " " + number;
            PriceLabel.Text = price.ToString();
            PickerLabel.ItemsSource = pickerCol;
            PickerLabel.SelectedItem = t;
            place = theSpot;
            typeOf = theType;
        }

        private void CloseModal(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }

        private async void SaveSpotData(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new Loading());
            var toUpdateSpot = (await fc
                .Child("Clients")
                .OnceAsync<ClubsList>()).ToList();

            var spotObject = toUpdateSpot.First(a => a.Object.place == place);

            await fc
            .Child("Clients")
            .Child(spotObject.Key)
            .Child("spotList")
            .Child((n - 1).ToString())
            .PutAsync(new OneSpot()
            {
                number = n,
                price = Convert.ToInt32(PriceLabel.Text),
                type = PickerLabel.SelectedItem.ToString()
            });
            await DisplayAlert("¡Exito!", txt + " " + n + " fué modificado correctamente", "Ok");
            await PopupNavigation.Instance.PopAllAsync();

        }


    }
}