using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnosFutbol.Models.Navigation;
using Firebase.Database.Query;
using Firebase.Database;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Services;

namespace TurnosFutbol.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlacePassword : PopupPage
    {
        private readonly FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");
        public event EventHandler<bool> CallbackEvent;
        public bool isTrue;
        public int placeId;
        public PlacePassword(int id)
        {
            InitializeComponent();
            CrossLabel.Text = "\uf00d";
            isTrue = false;
            placeId = id;
        }

        private void CloseModal(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }

        private async void checkPlacePass(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new Loading());

            var getPlaces = (await fc
            .Child("Clients")
            .OnceAsync<ClubsList>()).Select(item => new ClubsList
            {
                password = item.Object.password,
                id = item.Object.id,
            }).Where(a => a.id == placeId).ToList();

            foreach (var item in getPlaces)
            {
                if (item.password == thePassword.Text)
                {
                    isTrue = true;
                }
                else
                    isTrue = false;
            }

            await PopupNavigation.Instance.PopAllAsync();
        }

        private bool getBool()
        {
            return isTrue;
        }
        protected override void OnDisappearing() => CallbackEvent?.Invoke(this, getBool());

    }
}