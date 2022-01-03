using Firebase.Database;
using Firebase.Database.Query;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnosFutbol.Models.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TurnosFutbol.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OtherClassType : PopupPage
    {
        public int holdedShift;
        public string dayOfTheWeek;
        public string s;
        public FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");
        public OtherClassType(int shift_id, string weekDay, string type)
        {
            InitializeComponent();
            CrossLabel.Text = "\uf00d";
            s = type;
            dayOfTheWeek = weekDay;
            holdedShift = shift_id;
        }
        private void CloseModal(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }
        private async void SaveNewClassType(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new Loading());
            var toUpdateShift = (await fc
            .Child("Turnos")
            .Child(dayOfTheWeek.ToString())
            .Child(s)
            .OnceAsync<Shifts>()).ToList();

                        var shiftObject = toUpdateShift.First(a => a.Object.id == holdedShift);

                        await fc.Child("Turnos").Child(dayOfTheWeek.ToString()).Child(s).Child(shiftObject.Key).PutAsync(new Models.Navigation.Shifts()
                        {
                            persona = NewClassType.Text,
                            telefono = shiftObject.Object.telefono,
                            tipo = shiftObject.Object.tipo,
                            api_id = shiftObject.Object.api_id,
                            place = shiftObject.Object.place,
                            spot = shiftObject.Object.spot,
                            maxShifts = shiftObject.Object.maxShifts,
                            id = shiftObject.Object.id,
                            hora = shiftObject.Object.hora,
                            shiftFixed = shiftObject.Object.shiftFixed,
                        });

            await DisplayAlert("¡Exito!", "Se cambió el tipo de clase de manera correcta.", "Ok");
            await PopupNavigation.Instance.PopAllAsync();
        }

    }
}