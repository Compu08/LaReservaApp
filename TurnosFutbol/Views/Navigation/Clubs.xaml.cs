using Rg.Plugins.Popup.Services;
using Syncfusion.XForms.Graphics;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using Firebase.Database;
using Firebase.Database.Query;
using TurnosFutbol.Models.Navigation;
using TurnosFutbol.ViewModels.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TurnosFutbol.Views.Popups;

namespace TurnosFutbol.Views.Navigation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Clubs : ContentPage
    {
        private readonly FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");
        private readonly string s;
        private readonly string c;
        private int angle;
        private string textType;
        public SfLinearGradientBrush linearGradientBrush;
        public DateTime dateSelected;
        public ClubsList selectedClub;
        public Clubs(string city, string type)
        {
            InitializeComponent();
            dateSelected = DateTime.Now;
            datePicker.Date = DateTime.Now;
            datePicker.MaximumDate = DateTime.Now.AddDays(5);
            datePicker.MinimumDate = DateTime.Now;
            PopupNavigation.Instance.PushAsync(new Popups.Loading());
            //SfListView.ItemsSource = new ClubsViewModel().ClubsItem;
            titleName.Text = type;
            s = type;
            c = city;
            TextTypeConstructor();
            SfListView.BindingContext = new ClubsViewModel(type, city, textType);
            angle = 0;
            labelCity.Text = "Ciudad: " + city;
            ChangeColors();
            CleanShiftsPastDays(s);
            PopupNavigation.Instance.PopAsync();
        }

        private void GoBack(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void TextTypeConstructor()
        {
            switch (s)
            {
                case "Futbol":
                    textType = "canchas";
                    break;
                case "Tenis":
                    textType = "canchas";
                    break;
                case "Paddel":
                    textType = "canchas";
                    break;
                case "Restaurantes":
                    textType = "mesas";
                    break;
                case "Peluqueria":
                    textType = "sillas";
                    break;
                case "Gimnasios":
                    textType = "clases";
                    break;
            }
        }
        private void SfListView_ItemTapped(object sender, Syncfusion.ListView.XForms.ItemTappedEventArgs e)
        {
            selectedClub = e.ItemData as ClubsList;
            if ((e.ItemData as ClubsList).isLocked)
            {
                var popupPage = new PlacePassword((e.ItemData as ClubsList).id);
                popupPage.CallbackEvent += (object sender, bool r) => CallbackMethod(sender, r);
                PopupNavigation.Instance.PushAsync(popupPage);
            }
            else {
                if (selectedClub is ClubsList obj) Navigation.PushAsync(new ShiftsPage(obj.place, s, dateSelected, obj.timebefore, linearGradientBrush, (selectedClub).spotList));
            }
        }

        private void CallbackMethod(object sender, bool e)
        {
            if (e) { 
                if (selectedClub is ClubsList obj) Navigation.PushAsync(new ShiftsPage(obj.place, s, dateSelected, obj.timebefore, linearGradientBrush, (selectedClub).spotList));
            }
            else
                DisplayAlert("¡Error!", "La clave ingresada es incorrecta.", "Ok");
        }

        private void ChangeColors()
        {

            switch (s)
            {
                case "Futbol":
                    linearGradientBrush = new SfLinearGradientBrush
                    {
                        GradientStops = new Syncfusion.XForms.Graphics.GradientStopCollection(){
                    new SfGradientStop(){ Color = Color.FromHex("#49A800"), Offset=0.0},
                    new SfGradientStop(){ Color = Color.FromHex("#3B8205"), Offset=1.0},
                    }
                    };
                    gradient.BackgroundBrush = linearGradientBrush;
                    break;

                case "Tenis":
                    linearGradientBrush = new SfLinearGradientBrush
                    {
                        GradientStops = new Syncfusion.XForms.Graphics.GradientStopCollection(){
                    new SfGradientStop(){ Color = Color.FromHex("#F09819"), Offset=0.0},
                    new SfGradientStop(){ Color = Color.FromHex("#FF512F"), Offset=1.0},
                    }
                    };
                    gradient.BackgroundBrush = linearGradientBrush;
                    break;

                case "Paddel":
                    linearGradientBrush = new SfLinearGradientBrush
                    {
                        GradientStops = new Syncfusion.XForms.Graphics.GradientStopCollection(){
                    new SfGradientStop(){ Color = Color.FromHex("#5691c8"), Offset=0.0},
                    new SfGradientStop(){ Color = Color.FromHex("#457fca"), Offset=1.0},
                    }
                    };
                    gradient.BackgroundBrush = linearGradientBrush;
                    break;

                case "Peluqueria":
                    linearGradientBrush = new SfLinearGradientBrush
                    {
                        GradientStops = new Syncfusion.XForms.Graphics.GradientStopCollection(){
                    new SfGradientStop(){ Color = Color.FromHex("#f953c6"), Offset=0.0},
                    new SfGradientStop(){ Color = Color.FromHex("#b91d73"), Offset=1.0},
                    }
                    };
                    gradient.BackgroundBrush = linearGradientBrush;
                    break;

                case "Restaurantes":
                    linearGradientBrush = new SfLinearGradientBrush
                    {
                        GradientStops = new Syncfusion.XForms.Graphics.GradientStopCollection(){
                    new SfGradientStop(){ Color = Color.FromHex("#ED213A"), Offset=0.0},
                    new SfGradientStop(){ Color = Color.FromHex("#93291E"), Offset=1.0},
                    }
                    };
                    gradient.BackgroundBrush = linearGradientBrush;
                    break;

                case "Gimnasios":
                    linearGradientBrush = new SfLinearGradientBrush
                    {
                        GradientStops = new Syncfusion.XForms.Graphics.GradientStopCollection(){
                    new SfGradientStop(){ Color = Color.FromHex("#414345"), Offset=0.0},
                    new SfGradientStop(){ Color = Color.FromHex("#232526"), Offset=1.0},
                    }
                    };
                    gradient.BackgroundBrush = linearGradientBrush;
                    break;
            }


        }

        private void RefreshList(object sender, EventArgs e)
        {
            angle += 360;
            RefreshIcon.RotateTo(angle, 1000, Easing.SinInOut);
            PopupNavigation.Instance.PushAsync(new Popups.Loading());
            SfListView.BindingContext = new ClubsViewModel(s, c,textType);
            PopupNavigation.Instance.PopAsync();
        }

        private void DateTapped(object sender, EventArgs e)
        {
            if (Device.RuntimePlatform == "iOS")
            {
                var popupPage = new CalendarPopup(dateSelected);
                popupPage.CallbackEvent += (object sender, DateTime e) => CallbackMethod(sender, e);
                PopupNavigation.Instance.PushAsync(popupPage);
            }
            else {
                datePicker.Focus();
            }
        }

        private void CallbackMethod(object sender, DateTime e) {
            dateSelected = e;
            labelDate.Text = "\uf073 " + e.Day + "/" + e.Month + "/" + e.Year;
        }
        private void UpdateLabelDate(object sender, DateChangedEventArgs e)
        {
            DateTime d = datePicker.Date;
            dateSelected = datePicker.Date;
            labelDate.Text = "\uf073 " + d.Day + "/" + d.Month + "/" + d.Year;
        }
        private async void CleanShiftsPastDays(string type)
        {
            var yesterday = (int) DateTime.Today.AddDays(-1).DayOfWeek;
            var multiShiftsList = new SubShifts();

            var toUpdateShift = (await fc
                    .Child("Turnos")
                    .Child(yesterday.ToString())
                    .Child(type)
                    .OnceAsync<Shifts>()).ToList();

                foreach (var item in toUpdateShift)
                {
                    multiShiftsList.Clear();
                    var shiftTipo = 0;
                    var name = "DISPONIBLE";
                    var phone = "";
                    if (item.Object.tipo == 4)
                    {
                        shiftTipo = 4;
                        name = item.Object.persona;
                        phone = item.Object.telefono;
                    }
                    if (s == "Gimnasios" && item.Object.persona != "DISPONIBLE") {
                        name = item.Object.persona;
                    }

                if (item.Object.multiShifts != null) 
                    multiShiftsList = item.Object.multiShifts;

                    foreach (var subShift in multiShiftsList.ToList())
                    {
                        if (!subShift.fixedSub)
                        {
                            multiShiftsList.Remove(subShift);
                        }
                    }

                    await fc
                        .Child("Turnos")
                        .Child(yesterday.ToString())
                        .Child(type)
                        .Child(item.Key)
                        .PutAsync(new Shifts()
                        {
                            persona = name,
                            api_id = item.Object.api_id,
                            hora = item.Object.hora,
                            iconColor = item.Object.iconColor,
                            id = item.Object.id,
                            maxShifts = item.Object.maxShifts,
                            place = item.Object.place,
                            spot = item.Object.spot,
                            telefono = phone,
                            textColor = item.Object.textColor,
                            tipo = shiftTipo,
                            multiShifts = multiShiftsList,
                        });
                
            }
        }

        private void NewUpdateLabelDate(object sender, Syncfusion.SfCalendar.XForms.SelectionChangedEventArgs e)
        {

        }
    }
}
