using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
    public partial class HidePlace : ContentPage
    {
        private ObservableCollection<ClubsList> paymentsItems;

        private ObservableCollection<ClubsList> PaymentsItems
        {
            get { return paymentsItems; }
            set
            {
                paymentsItems = value;
                OnPropertyChanged();
            }
        }

        private readonly FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");

        public HidePlace()
        {
            InitializeComponent();
            PaymentsItems = new ObservableCollection<ClubsList>();
            ChargePlacesList();
        }
        private void GoBack(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
        private async void ChargePlacesList()
        {
            await PopupNavigation.Instance.PushAsync(new Loading());

            PaymentsItems = new ObservableCollection<ClubsList>();
            var toList = (await fc
                .Child("Clients")
                .OnceAsync<ClubsList>()).Select(item => new ClubsList
            {
                id = item.Object.id,
                place = item.Object.place,
                price = item.Object.price,
                payment = Convert.ToBoolean(item.Object.payment),
                hide = Convert.ToBoolean(item.Object.hide),
                type = item.Object.type,
                seller = item.Object.seller,
            }).ToList().OrderBy(i => i.place);

            foreach (ClubsList item in toList)
            {
                var paymentText = "\uf00d";
                Color paymentColor = Color.Red;
                var hideText = "\uf070";
                Color hideColor = Color.Gray;
                if (item.payment == true)
                {
                    paymentText = "\uf00c";
                    paymentColor = Color.Green;
                }

                if (item.hide == true)
                {
                    hideText = "\uf070";
                    hideColor = Color.Red;
                }

                PaymentsItems.Add(new ClubsList()
                {
                    id=item.id,place = item.place, price = "$" + item.price, seller = item.seller, type = item.type,
                    paymentT = paymentText, paymentC = paymentColor, hideT = hideText, hideC = hideColor
                });
            }

            lstview.BindingContext = this.BindingContext;
            lstview.ItemsSource = PaymentsItems;
            await PopupNavigation.Instance.PopAsync();
        }

        private async void PickedItem(object sender, SelectedItemChangedEventArgs e)
        {
            var theItem = e.SelectedItem as ClubsList;
            if (Preferences.Get("user", "") == "Gaston Hernandez")
            {
                bool answer = await DisplayAlert("Seleccionar Acción", "¿Que acción desea realizar?", "Ocultar", "Ver Seller");
                if (answer)
                {
                    if (theItem != null) HideThePlace(theItem.id);
                }
                else
                {
                    if (theItem != null) await DisplayAlert("Ver Seller", "El vendedor es: " + theItem.seller, "Ok");
                }
            }
            else
            {
                if (theItem != null) HideThePlace(theItem.id);
            }
        }

        private async void HideThePlace(int placeId)
        {
            await PopupNavigation.Instance.PushAsync(new Loading());
            
            var toUpdatePlaceList = (await fc
                .Child("Clients")
                .OnceAsync<Models.Navigation.ClubsList>()).ToList();

            var placeObject = toUpdatePlaceList.First(a => a.Object.id == placeId);

            await fc.Child("Clients").Child(placeObject.Key).PutAsync(new ClubsList()
            {
                id = placeObject.Object.id,
                place = placeObject.Object.place,
                price = placeObject.Object.price,
                payment = Convert.ToBoolean(placeObject.Object.payment),
                hide = (!placeObject.Object.hide),
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
            });


            await PopupNavigation.Instance.PopAsync();

            ChargePlacesList();
        }


    }
}