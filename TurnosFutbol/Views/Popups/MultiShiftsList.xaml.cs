using Firebase.Database;
using Firebase.Database.Query;
using Plugin.Messaging;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnosFutbol.Models.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.OpenWhatsApp;
using Xamarin.Forms.Xaml;

namespace TurnosFutbol.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MultiShiftsList : PopupPage
    {
        private readonly FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");
        public Models.Navigation.Spots shiftsList;
        public bool allowSwitch;
        public int dayOfWeek;
        public string t;
        public int idShift;
        public MultiShiftsList(SubShifts ObjList, string day, string hora, int dayOfTheWeek, string type, int shiftId)
        {
            InitializeComponent();
            allowSwitch = true;
            t = type;
            dayOfWeek = dayOfTheWeek;
            idShift = shiftId;
            topText.Text = "Turno día " + day;
            labelSpot.Text = hora+" hs.";
            CrossLabel.Text = "\uf00d";
            lstView.ItemsSource = ObjList;

                if (ObjList == null)
                {
                    lstView.IsVisible = false;
                    LabelEmpty.IsVisible = true;
                }
            

        }

        private void CloseModal(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }

        private void lstView_SwipeEnded(object sender, Syncfusion.ListView.XForms.SwipeEndedEventArgs e)
        {
            var selectedObj = e.ItemData as SubShift;
            if (allowSwitch)
            {
                if (e.SwipeDirection == Syncfusion.ListView.XForms.SwipeDirection.Right)
                    Chat.Open("54" + selectedObj.telefonoSub, "");
                else
                {
                    CrossMessaging.Current.PhoneDialer.MakePhoneCall("+54" + selectedObj.telefonoSub);
                }
            }

            this.lstView.ResetSwipe();
        }

        private void lstView_SwipeStarted(object sender, Syncfusion.ListView.XForms.SwipeStartedEventArgs e)
        {
            try
            {
                if ((e.ItemData as SubShift).telefonoSub.Length > 0)
                    return;
            }
            catch
            {
                allowSwitch = false;
                DisplayAlert("¡Error!", "El Cliente no brindó su número telefónico.", "Ok");
            }
        }

        private async void lstView_ItemTapped(object sender, Syncfusion.ListView.XForms.ItemTappedEventArgs e)
        {
            var obj = (SubShift)e.ItemData;
            var idSub = obj.idSub;
        
            var answer = await DisplayAlert("Eliminar Turno", "¿Está seguro que desea eliminar el turno de " + obj.personaSub + "?", "Si", "No");

            if (answer) {
                UpdateSubShifts(idSub,1);
                await DisplayAlert("¡Exito!", "Se ha eliminado el turno de manera correcta.", "Ok");
            }

        }

        private async void UpdateSubShifts(int id, int actionType) {
            await PopupNavigation.Instance.PushAsync(new Loading());
            
            var toDelete = (await fc
                .Child("Turnos")
                .Child(dayOfWeek.ToString())
                .Child(t)
                .OnceAsync<Shifts>()).FirstOrDefault(a => a.Object.id == idShift);

            var multiShiftsNewList = new SubShifts();

            if(toDelete.Object.multiShifts != null)
                multiShiftsNewList = toDelete.Object.multiShifts;

            ///ACTIONS: 1 = DELETE / 2 = FIXED SHIFT
            if (actionType == 1)
            {
                foreach (var item in multiShiftsNewList.ToList())
                {
                    if (item.idSub == id)
                        multiShiftsNewList.Remove(item);
                }
            }
            else if (actionType == 2) {
                foreach (var item in multiShiftsNewList.ToList())
                {
                    if (item.idSub == id)
                        item.fixedSub = !item.fixedSub;
                }
            }

            await fc
                .Child("Turnos")
                .Child(dayOfWeek.ToString())
                .Child(t)
                .Child(toDelete.Key)
                .PutAsync(new Shifts()
                {
                    persona = toDelete.Object.persona,
                    api_id = toDelete.Object.api_id,
                    hora = toDelete.Object.hora,
                    iconColor = toDelete.Object.iconColor,
                    id = toDelete.Object.id,
                    maxShifts = toDelete.Object.maxShifts,
                    place = toDelete.Object.place,
                    spot = toDelete.Object.spot,
                    telefono = toDelete.Object.telefono,
                    textColor = toDelete.Object.textColor,
                    tipo = toDelete.Object.tipo,
                    multiShifts = multiShiftsNewList,
                });

            ///REFRESH THE LISTVIEW WITH THE NEW DATA, If has data, refresh, else pop the popup
            if (multiShiftsNewList.Count > 0)
            {
                lstView.ItemsSource = multiShiftsNewList;
                await PopupNavigation.Instance.PopAsync();
            }
            else {
                await PopupNavigation.Instance.PopAllAsync();
            }

        }

        private async void lstView_ItemHolding(object sender, ItemHoldingEventArgs e)
        {
            var obj = (SubShift)e.ItemData;

            if (!obj.fixedSub)
            {
                bool answerPermanent = await DisplayAlert("Tipo de Turno", "¿Quieres fijar permanentemente el turno de " + obj.personaSub + " a las " + labelSpot.Text + "?", "Si", "No");
                if (answerPermanent)
                {
                    ///Admin wants to make the shift permanent
                    UpdateSubShifts(obj.idSub, 2);
                    await DisplayAlert("¡Exito!", topText.Text + " a las " + labelSpot.Text + " fué fijado", "Ok");
                }
            }
            else {
                bool exitPermanent = await DisplayAlert("Tipo de Turno", "¿Quieres que el turno de "+obj.personaSub+" a las "+ labelSpot.Text + " ya no sea fijo?", "Si", "No");
                if(exitPermanent)
                    UpdateSubShifts(obj.idSub, 2);
                    await DisplayAlert("¡Exito!", topText.Text + " a las " + labelSpot.Text + " ya no se encuentra fijado", "Ok");
            }
        }

        private void PushAddNewShift(object sender, EventArgs e)
        {
            var popupPage = new AddNewShiftAsMultiShift(dayOfWeek, t, idShift);
            popupPage.CallbackEvent += (object sender, SubShifts e) => CallbackMethod(sender, e);
            PopupNavigation.Instance.PushAsync(popupPage);
        }

        private void CallbackMethod(object sender, SubShifts e)
        {
            lstView.ItemsSource = e;   
        }

    }
}