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
    public partial class DeletePlace : ContentPage
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

        public DeletePlace()
        {
            InitializeComponent();
            PaymentsItems = new ObservableCollection<ClubsList>();
            ChargePlacesList();
        }

        private void GoBack(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private async void PickedItem(object sender, SelectedItemChangedEventArgs e)
        {
            var theItem = e.SelectedItem as ClubsList;

            bool answer = await DisplayAlert("Seleccionar Acción",
                "¿Está seguro que desea eliminar " + theItem.place +
                " ? Este proceso es irreversible y se borrará toda información relacionada", "Sí",
                "No");
            if (answer)
            {
                if (theItem != null) DeleteThePlace(theItem.id);
            }
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

                PaymentsItems.Add(new ClubsList()
                {
                    id = item.id,
                    place = item.place,
                    price = "$" + item.price,
                    seller = item.seller,
                    type = item.type,
                    hideT = "\uf2ed",
                    hideC = Color.Red
                });
            }

            lstview.BindingContext = this.BindingContext;
            lstview.ItemsSource = PaymentsItems;
            await PopupNavigation.Instance.PopAsync();
        }

        private async void DeleteThePlace(int placeId)
        {
            await PopupNavigation.Instance.PushAsync(new Loading());

            var toDeletePlace = (await fc
                .Child("Clients")
                .OnceAsync<Models.Navigation.ClubsList>()).FirstOrDefault(a => a.Object.id == placeId);

            await fc.Child("Clients").Child(toDeletePlace?.Key).DeleteAsync();

            var toDeletePlace2 = (await fc
                .Child(toDeletePlace?.Object.type)
                .OnceAsync<Models.Navigation.ClubsList>()).FirstOrDefault(a => a.Object.id == placeId);

            await fc.Child(toDeletePlace?.Object.type).Child(toDeletePlace2?.Key).DeleteAsync();

            await DisplayAlert("¡Exito!", "Se eliminó "+toDeletePlace?.Object.place, "Ok");

            await PopupNavigation.Instance.PopAsync();
        }

    }
}