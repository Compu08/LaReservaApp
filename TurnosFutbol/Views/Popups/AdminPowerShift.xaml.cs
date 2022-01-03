using Firebase.Database;
using Firebase.Database.Query;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms.Xaml;

namespace TurnosFutbol.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdminPowerShift : PopupPage
    {
        public int dayOfTheWeek;
        public string s;
        public int pickedId;
        public string hour;
        public string selectedD;
        public FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");
        public AdminPowerShift(int shiftId, string type, int day, string selectedDay, string hora)
        {
            InitializeComponent();
            CrossLabel.Text = "\uf00d";
            dayOfTheWeek = day;
            s = type;
            pickedId = shiftId;
            selectedD = selectedDay;
            hour = hora;
        }

        private void CloseModal(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }

        private async void Save_Shift(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new Popups.Loading());
            UpdateShift();
            await PopupNavigation.Instance.PopAsync();
        }

        private async void UpdateShift()
        {
            List<FirebaseObject<Models.Navigation.Shifts>> toUpdateShift = (await fc
               .Child("Turnos")
               .Child(dayOfTheWeek.ToString())
               .Child(s)
               .OnceAsync<Models.Navigation.Shifts>()).ToList();

            FirebaseObject<Models.Navigation.Shifts> shiftObject = toUpdateShift.Where(a => a.Object.id == pickedId).First();

            await fc.Child("Turnos").Child(dayOfTheWeek.ToString()).Child(s).Child(shiftObject.Key).PutAsync(new Models.Navigation.Shifts()
            {
                persona = shiftName.Text,
                telefono = shiftPhone.Text,
                tipo = 1,
                api_id = "",
                place = shiftObject.Object.place,
                spot = shiftObject.Object.spot,
                maxShifts = shiftObject.Object.maxShifts,
                id = shiftObject.Object.id,
                hora = shiftObject.Object.hora
            });

            await DisplayAlert("¡Exito!", "Turno Reservado para el día " + selectedD + " a las " + hour + " hs.", "Ok");
            await PopupNavigation.Instance.PopAsync();
        }
    }

}