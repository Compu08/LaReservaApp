using Firebase.Database;
using Firebase.Database.Query;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Syncfusion.SfGauge.XForms;
using Syncfusion.SfPicker.XForms;
using TurnosFutbol.Controls;
using TurnosFutbol.Models.Dashboard;
using TurnosFutbol.Models.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;

namespace TurnosFutbol.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddNewShift : PopupPage
    {
        FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");
        public ObservableCollection<object> SelectedTime;
        public int day;
        public string _place;
        public string _sport;
        public bool finished;
        public string _text;
        public int _spots;
        public string peopleString;
        public CustomTimePicker pickerTime;
        public ObservableCollection<object> todaysTime;
        public AddNewShift(int selectedDay, string sport, string place, int spots, string text)
        {
            InitializeComponent();
            todaysTime = new ObservableCollection<object>();
            day = selectedDay;
            _sport = sport;
            _place = place;
            _text = text;
            _spots = spots;
            PickerConstructor();
        }

        private void PickerConstructor()
        {
            var daytime = DateTime.Now;
            daytime.ToString("dd/mm/yyy HH:mm:ss");
            todaysTime.Add(daytime.Hour.ToString());
            todaysTime.Add("0");
            SelectedTime = todaysTime;

            var helper = 0;
            var aux = 0;

            while (helper != _spots)
            {
                aux = helper + 1;
                helper++;
            }

            pickerTime = new CustomTimePicker(_text, aux)
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                PickerHeight = 400,
                PickerMode = PickerMode.Default,
                PickerWidth = 300,
                HeaderBackgroundColor = Color.FromHex("19478F"),
                HeaderTextColor = Color.White,
                CancelButtonTextColor = Color.FromHex("19478F"),
                OKButtonTextColor = Color.FromHex("19478F"),
                SelectedItemTextColor = Color.FromHex("19478F"),
                SelectedItem = SelectedTime
            };
            pickerTime.CancelButtonClicked += GoBack;
            pickerTime.OkButtonClicked += AddShiftClicked;
            pickerTime.IsOpen = !pickerTime.IsOpen;
            pickerTime.EnableLooping = false;

            MainStackLayout.Children.Add(pickerTime);            

            if (_sport == "Gimnasios")
            {
                _text = "para";
                peopleString = "personas";
            }
            else
            {
                _text = "en " + _text;
            }
        }

        private void GoBack(object sender, Syncfusion.SfPicker.XForms.SelectionChangedEventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }

        private async void AddShiftClicked(object sender, Syncfusion.SfPicker.XForms.SelectionChangedEventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new Popups.Loading());
            Random rnd = new Random();
            int randomShiftId = rnd.Next(1, 9999999);
            ObservableCollection<object> selectedItem = pickerTime.SelectedItem as ObservableCollection<object>;
            string hour = selectedItem[0].ToString() + ":" + selectedItem[1].ToString();
            await AddShift(randomShiftId, hour, Convert.ToInt32(selectedItem[2]));
            await PopupNavigation.Instance.PopAsync();
            await DisplayAlert("¡Exito!", "Se añadió correctamente el turno de las " + hour + " hs. "+_text+" "+selectedItem[2].ToString()+" "+peopleString, "Ok");
            finished = true;
            await PopupNavigation.Instance.PopAsync();
        }
        public async Task AddShift(int shiftId, string hour, int selectedSpot)
        {
            if (_sport == "Gimnasios")
            {
                var emptyMultiShifts = new SubShifts();

                await fc
                  .Child("Turnos")
                  .Child(day.ToString())
                  .Child(_sport)
                  .PostAsync(new Shifts() { persona = "DISPONIBLE", id = shiftId, hora = hour, spot = 1, place = _place, tipo = 0, multiShifts = emptyMultiShifts, maxShifts = selectedSpot });                
            }
            else { 
            await fc
              .Child("Turnos")
              .Child(day.ToString())
              .Child(_sport)
              .PostAsync(new Shifts() { persona = "DISPONIBLE", id = shiftId, hora = hour, spot = selectedSpot, place = _place, tipo = 0 });
            }
        }

    }
}