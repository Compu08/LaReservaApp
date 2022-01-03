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
    public partial class VipMembers : ContentPage
    {
        private ObservableCollection<ClubsList> vipPlaces;

        private ObservableCollection<ClubsList> VipPlaces
        {
            get { return vipPlaces; }
            set
            {
                vipPlaces = value;
                OnPropertyChanged();
            }
        }

        private readonly FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");

        public VipMembers()
        {
            InitializeComponent();
            ChargePlacesList();
        }

        private void GoBack(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
        private async void ChargePlacesList()
        {
            await PopupNavigation.Instance.PushAsync(new Loading());

            VipPlaces = new ObservableCollection<ClubsList>();
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
                vip = item.Object.vip,
            }).ToList().OrderBy(i => i.place);

            foreach (ClubsList item in toList)
            {
                var hideText = "\uf521";
                Color hideColor = Color.Gray;

                if (item.vip == true)
                {
                    hideText = "\uf521";
                    hideColor = Color.FromHex("#ffb300");
                }

                VipPlaces.Add(new ClubsList()
                {
                    id = item.id,
                    place = item.place,
                    price = "$" + item.price,
                    seller = item.seller,
                    type = item.type,
                    hideT = hideText,
                    hideC = hideColor,
                    vip = item.vip
                });
            }

            lstview.BindingContext = this.BindingContext;
            lstview.ItemsSource = VipPlaces;
            await PopupNavigation.Instance.PopAsync();
        }

        private void PickedItem(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is ClubsList theItem) SetVip(theItem.id);
        }

        private async void SetVip(int placeId)
        {
            await PopupNavigation.Instance.PushAsync(new Loading());

            var toUpdatePlaceList = (await fc
                .Child("Clients")
                .OnceAsync<Models.Navigation.ClubsList>()).ToList();

            var placeObject = toUpdatePlaceList.First(a => a.Object.id == placeId);

            var newPrice = Convert.ToInt32(placeObject.Object.price);
            
            if (placeObject.Object.vip) newPrice -= 1000; else newPrice += 1000;
            

            await fc.Child("Clients").Child(placeObject.Key).PutAsync(new ClubsList()
            {
                id = placeObject.Object.id,
                place = placeObject.Object.place,
                price = newPrice.ToString(),
                payment = Convert.ToBoolean(placeObject.Object.payment),
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
                vip = !(placeObject.Object.vip),
                timebefore = placeObject.Object.timebefore,
                spotList = placeObject.Object.spotList,
            });


            await PopupNavigation.Instance.PopAsync();

            ChargePlacesList();
        }
    }
}