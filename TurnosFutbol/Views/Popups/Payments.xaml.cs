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
    public partial class Payments : ContentPage
    {
        private int totalPlaces;
        private int totalCollect;
        private int totalCash;
        private ObservableCollection<ClubsList> paymentsItems;
        private ObservableCollection<ClubsList> PaymentsItems;
        private readonly FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");
        public Payments()
        {
            InitializeComponent();
            ChargePaymentsList();
        }
        private void GoBack(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private async void ChargePaymentsList()
        {
            await PopupNavigation.Instance.PushAsync(new Loading());
            totalCollect = 0;
            totalPlaces = 0;
            totalCash = 0;
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
                courts = item.Object.courts
            }).ToList().OrderBy(i => i.place);

            foreach (ClubsList item in toList)
            {
                var paymentText = "\uf058";
                Color paymentColor = Color.Gray;
                if (item.payment == true)
                {
                    paymentText = "\uf058";
                    paymentColor = Color.Green;
                }

                PaymentsItems.Add(new ClubsList()
                {
                    id = item.id,
                    place = item.place,
                    price = "$" + item.price,
                    seller = item.seller,
                    type = item.type,
                    paymentT = paymentText,
                    paymentC = paymentColor,
                    hideT = item.hideT,
                    hideC = item.hideC
                });

                totalPlaces += 1;
                totalCash = totalCash+Convert.ToInt32(item.price);
                if(!item.payment) totalCollect = totalCollect+Convert.ToInt32(item.price);
            }

            lstview.BindingContext = this.BindingContext;
            lstview.ItemsSource = PaymentsItems;

            UpdateResumesLabels();
            await PopupNavigation.Instance.PopAsync();
        }

        private void UpdateResumesLabels()
        {
            ResumePlaces.Text = "Lugares: " + totalPlaces;
            ResumeToCollect.Text = "Por cobrar: $" + totalCollect;
            ResumeTotal.Text = "TOTAL: $" + totalCash;
        }

        private async void SetPayment(int placeId)
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
                hide = placeObject.Object.hide,
                payment =!(placeObject.Object.payment),
                paymentC = placeObject.Object.paymentC,
                type = placeObject.Object.type,
                seller = placeObject.Object.seller,
                adress = placeObject.Object.adress,
                city = placeObject.Object.city,
                contacto = placeObject.Object.contacto,
                courts = placeObject.Object.courts,
                hideC = placeObject.Object.hideC,
                phone = placeObject.Object.phone,
                picture = placeObject.Object.picture,
                value = placeObject.Object.value,
                vip = placeObject.Object.vip,
                spotList = placeObject.Object.spotList,
            });

            var paymentPrice = Convert.ToInt32(placeObject.Object.price);
            if (placeObject.Object.payment) totalCollect -= paymentPrice; else totalCollect += paymentPrice;
            await PopupNavigation.Instance.PopAsync();

            ChargePaymentsList();
        }

        private void PickedItem(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is ClubsList theItem) SetPayment(theItem.id);
        }

        private async void ResetPayments(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("Confirmar acción", "¿Estas seguro de reiniciar todos los pagos del mes?", "Si", "No");
            if (!answer) return;

            await PopupNavigation.Instance.PushAsync(new Loading());

            var toUpdatePlacesList = (await fc
                .Child("Clients")
                .OnceAsync<Models.Navigation.ClubsList>()).ToList();

            var falseObjects = toUpdatePlacesList.Where(a => a.Object.payment == true).ToList();

            foreach (var item in falseObjects)
            {
                await fc.Child("Clients").Child(item.Key).PutAsync(new ClubsList()
                {
                    id = item.Object.id,
                    place = item.Object.place,
                    price = item.Object.price,
                    hide = item.Object.hide,
                    payment = !(item.Object.payment),
                    paymentC = item.Object.paymentC,
                    type = item.Object.type,
                    seller = item.Object.seller,
                    adress = item.Object.adress,
                    city = item.Object.city,
                    contacto = item.Object.contacto,
                    courts = item.Object.courts,
                    hideC = item.Object.hideC,
                    phone = item.Object.phone,
                    picture = item.Object.picture,
                    value = item.Object.value,
                    vip = item.Object.vip,
                    spotList = item.Object.spotList,
                });
            }

            totalCollect = totalCash;
            await PopupNavigation.Instance.PopAsync();

            ChargePaymentsList();
        }
    }
}