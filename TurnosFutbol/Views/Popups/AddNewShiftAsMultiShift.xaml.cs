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
	public partial class AddNewShiftAsMultiShift : PopupPage
	{
        private readonly FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");
        public event EventHandler<SubShifts> CallbackEvent;
        private int idShift;
        private int dayOfWeek;
        private string t;
        public SubShifts multiShiftsNewList;
        public AddNewShiftAsMultiShift (int DayOfTheWeek, string type, int ShiftId)
		{
            idShift = ShiftId;
            dayOfWeek = DayOfTheWeek;
            t = type;
            multiShiftsNewList = new SubShifts();
			InitializeComponent ();
			CrossLabel.Text = "\uf00d";
		}
		private void CloseModal(object sender, EventArgs e)
		{
			PopupNavigation.Instance.PopAsync();
		}

        protected override void OnDisappearing() => CallbackEvent?.Invoke(this, multiShiftsNewList);

        private async void SaveNewShift(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new Loading());

            var toUpdate = (await fc
                .Child("Turnos")
                .Child(dayOfWeek.ToString())
                .Child(t)
                .OnceAsync<Shifts>()).FirstOrDefault(a => a.Object.id == idShift);

            if(toUpdate.Object.multiShifts != null)
                multiShiftsNewList = toUpdate.Object.multiShifts;

            var random = new Random();
            var randomId = random.Next(0, 99999);

            multiShiftsNewList.Add(new SubShift { personaSub = ShiftName.Text, telefonoSub = ShiftPhone.Text, fixedSub = isOn.IsChecked, idSub = randomId, itemIndex = 0 });

            string newName = toUpdate.Object.persona;
            int tipo = 0;

            if (toUpdate.Object.maxShifts == multiShiftsNewList.Count) { 
                newName = "OCUPADO";
                tipo = 1;
            }

            await fc
                .Child("Turnos")
                .Child(dayOfWeek.ToString())
                .Child(t)
                .Child(toUpdate.Key)
                .PutAsync(new Shifts()
                {
                    persona = newName,
                    api_id = toUpdate.Object.api_id,
                    hora = toUpdate.Object.hora,
                    iconColor = toUpdate.Object.iconColor,
                    id = toUpdate.Object.id,
                    maxShifts = toUpdate.Object.maxShifts,
                    place = toUpdate.Object.place,
                    spot = toUpdate.Object.spot,
                    telefono = toUpdate.Object.telefono,
                    textColor = toUpdate.Object.textColor,
                    tipo = tipo,
                    multiShifts = multiShiftsNewList,
                });

            await DisplayAlert("¡Exito!", "Se agregó correctamente el turno asignado a " + ShiftName.Text, "Ok");

            await PopupNavigation.Instance.PopAsync();
            await PopupNavigation.Instance.PopAsync();

        }

    }
}