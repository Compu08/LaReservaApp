using Firebase.Database;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Firebase.Database.Query;
using TurnosFutbol.Models.Navigation;
using TurnosFutbol.ViewModels.Navigation;
using TurnosFutbol.Views.Popups;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TurnosFutbol.Views.Navigation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BossPage : ContentPage
    {

        private ObservableCollection<ClubsList> paymentsItems;
        private ObservableCollection<ClubsList> PaymentsItems { get { return paymentsItems; } set { paymentsItems = value; OnPropertyChanged(); } }

        private readonly FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");
        private int id;
        private bool paymentBool;
        
        public BossPage()
        {
            InitializeComponent();
            PaymentsItems = new ObservableCollection<ClubsList>();
        }

        private void GoBack(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void PushAddClubPage(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new AddPlace());
        }
        private void PushAddCity(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new AddCity());
        }
        private void PushHideLabel(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new HidePlace());
        }
        private void PushDeleteLabel(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new DeletePlace());
        }
        private void PushAdminsDifusion(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new SendMessage(true));
        }

        private async void PaymentClicked(object sender, EventArgs e)
        {

            var toUpdateClient = (await fc
                .Child("Clients")
                .OnceAsync<ClubsList>()).First(a => a.Object.id == id);

            await fc
                .Child("Clients")
                .Child(toUpdateClient.Key)
                .PutAsync(new ClubsList() { payment = !paymentBool, price = toUpdateClient.Object.price, place=toUpdateClient.Object.place,type = toUpdateClient.Object.type,id=toUpdateClient.Object.id,hide = toUpdateClient.Object.hide});
        }

        private void Lstview_OnItemSelected(object sender, ItemTappedEventArgs e)
        {
            id = ((ClubsList) e.Item).id;
            paymentBool = ((ClubsList) e.Item).payment;
        }

        private void NewPasswordClicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new PasswordCreator());
        }

        private void PaymentsClicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new Payments());
        }

        private void VipMembersClicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new VipMembers());
        }
    }
}