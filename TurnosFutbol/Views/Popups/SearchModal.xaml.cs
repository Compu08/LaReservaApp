using Firebase.Database;
using Rg.Plugins.Popup.Services;
using Syncfusion.XForms.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TurnosFutbol.Models.Navigation;
using TurnosFutbol.Views.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TurnosFutbol.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchModal : ContentPage
    {
        public ClubsList selectedClub;
        public SfLinearGradientBrush linearGradientBrush;
        public DateTime dateSelected;
        public string tT;
        private readonly FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");
        public ObservableCollection<ClubsList> clubsItem;
        public ObservableCollection<ClubsList> ClubsItem { get { return clubsItem; } set { clubsItem = value; OnPropertyChanged(); } }
        public SearchModal()
        {
            InitializeComponent();
            ClubsItem = new ObservableCollection<ClubsList>();
            dateSelected = DateTime.Now;
            GetAllData();
        }

        private void GoBack(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void theSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                SfListView.IsVisible = false;
                NotFoundLabel.IsVisible = false;
                SfListView.ItemsSource = ClubsItem;
                LogoBackground.IsVisible = true;
            }

            else
            {
                var result = ClubsItem.Where(x => x.place.ToLower().Contains(e.NewTextValue.ToLower()));
                //DisplayAlert("ok",result.Count().ToString(), "ok");
                if (result.Count() > 0)
                {
                    LogoBackground.IsVisible = false;
                    NotFoundLabel.IsVisible = false;
                    SfListView.ItemsSource = result;
                    SfListView.IsVisible = true;
                }
                else if(result.Count() <= 0){
                    SfListView.IsVisible = false;
                    LogoBackground.IsVisible = false;
                    NotFoundLabel.IsVisible = true;
                }
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
            else
            {
                if (selectedClub is ClubsList obj) Navigation.PushAsync(new ShiftsPage(obj.place, obj.type, dateSelected, obj.timebefore, ChangeColors(), (selectedClub).spotList));
            }
        }

        private void CallbackMethod(object sender, bool e)
        {
            if (e)
            {
                if (selectedClub is ClubsList obj) Navigation.PushAsync(new ShiftsPage(obj.place, obj.type, dateSelected, obj.timebefore, ChangeColors(), (selectedClub).spotList));
            }
            else
                DisplayAlert("¡Error!", "La clave ingresada es incorrecta.", "Ok");
        }

        private string CapitalizeText(string str)
        {
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        private async void GetAllData() {
            await PopupNavigation.Instance.PushAsync(new Loading());

            var getPlaces = (await fc
            .Child("Clients")
            .OnceAsync<ClubsList>()).Select(item => new ClubsList
            {
                id = item.Object.id,
                place = item.Object.place,
                picture = item.Object.picture,
                adress = item.Object.adress,
                value = "Turno: $" + item.Object.value,
                courts = CapitalizeText(GetPlaceText(item.Object.type)) + ": " + item.Object.courts,
                city = item.Object.city,
                phone = item.Object.phone,
                hide = item.Object.hide,
                type = item.Object.type,
                vip = item.Object.vip,
                timebefore = item.Object.timebefore,
                spotList = item.Object.spotList,
                isLocked = item.Object.isLocked,
                password = item.Object.password,
            }).ToList();

            foreach (var item in getPlaces) { 
                if(item.hide == false)
                    ClubsItem.Add(item);
            }

            await PopupNavigation.Instance.PopAsync();
        }
        
        public string GetPlaceText(string type)
        {
            switch (type) {
                case "Futbol":
                    tT = "Canchas";
                    return tT;
                case "Tenis":
                    tT = "Canchas";
                    return tT;
                case "Paddel":
                    tT = "Canchas";
                    return tT;
                case "Restaurantes":
                    tT = "Mesas";
                    return tT;
                case "Peluquerias":
                    tT = "Sillas";
                    return tT;
                case "Gimnasios":
                    tT = "Clases";
                    return tT;
                default:
                    tT = "Turnos";
                    return tT;
            }
        }   
        private SfLinearGradientBrush ChangeColors()
        {

            switch (selectedClub.type)
            {
                case "Futbol":
                    linearGradientBrush = new SfLinearGradientBrush
                    {
                        GradientStops = new Syncfusion.XForms.Graphics.GradientStopCollection(){
                    new SfGradientStop(){ Color = Color.FromHex("#49A800"), Offset=0.0},
                    new SfGradientStop(){ Color = Color.FromHex("#3B8205"), Offset=1.0},
                    }
                    };
                    return linearGradientBrush;

                case "Tenis":
                    linearGradientBrush = new SfLinearGradientBrush
                    {
                        GradientStops = new Syncfusion.XForms.Graphics.GradientStopCollection(){
                    new SfGradientStop(){ Color = Color.FromHex("#F09819"), Offset=0.0},
                    new SfGradientStop(){ Color = Color.FromHex("#FF512F"), Offset=1.0},
                    }
                    };
                    return linearGradientBrush;

                case "Paddel":
                    linearGradientBrush = new SfLinearGradientBrush
                    {
                        GradientStops = new Syncfusion.XForms.Graphics.GradientStopCollection(){
                    new SfGradientStop(){ Color = Color.FromHex("#143bff"), Offset=0.0},
                    new SfGradientStop(){ Color = Color.FromHex("#0d1d71"), Offset=1.0},
                    }
                    };
                    return linearGradientBrush;

                case "Peluqueria":
                    linearGradientBrush = new SfLinearGradientBrush
                    {
                        GradientStops = new Syncfusion.XForms.Graphics.GradientStopCollection(){
                    new SfGradientStop(){ Color = Color.FromHex("#f953c6"), Offset=0.0},
                    new SfGradientStop(){ Color = Color.FromHex("#b91d73"), Offset=1.0},
                    }
                    };
                    return linearGradientBrush;

                case "Restaurantes":
                    linearGradientBrush = new SfLinearGradientBrush
                    {
                        GradientStops = new Syncfusion.XForms.Graphics.GradientStopCollection(){
                    new SfGradientStop(){ Color = Color.FromHex("#ED213A"), Offset=0.0},
                    new SfGradientStop(){ Color = Color.FromHex("#93291E"), Offset=1.0},
                    }
                    };
                    return linearGradientBrush;

                case "Gimnasios":
                    linearGradientBrush = new SfLinearGradientBrush
                    {
                        GradientStops = new Syncfusion.XForms.Graphics.GradientStopCollection(){
                    new SfGradientStop(){ Color = Color.FromHex("#414345"), Offset=0.0},
                    new SfGradientStop(){ Color = Color.FromHex("#232526"), Offset=1.0},
                    }
                    };
                    return linearGradientBrush;

                case "Lavadero":
                    linearGradientBrush = new SfLinearGradientBrush
                    {
                        GradientStops = new Syncfusion.XForms.Graphics.GradientStopCollection(){
                    new SfGradientStop(){ Color = Color.FromHex("#17bf8a"), Offset=0.0},
                    new SfGradientStop(){ Color = Color.FromHex("#117c5a"), Offset=1.0},
                    }
                    };
                    return linearGradientBrush;

                case "Squash":
                    linearGradientBrush = new SfLinearGradientBrush
                    {
                        GradientStops = new Syncfusion.XForms.Graphics.GradientStopCollection(){
                    new SfGradientStop(){ Color = Color.FromHex("#414345"), Offset=0.0},
                    new SfGradientStop(){ Color = Color.FromHex("#232526"), Offset=1.0},
                    }
                    };
                    return linearGradientBrush;

                case "Salud":
                    linearGradientBrush = new SfLinearGradientBrush
                    {
                        GradientStops = new Syncfusion.XForms.Graphics.GradientStopCollection(){
                    new SfGradientStop(){ Color = Color.FromHex("#5691c8"), Offset=0.0},
                    new SfGradientStop(){ Color = Color.FromHex("#457fca"), Offset=1.0},
                    }
                    };
                    return linearGradientBrush;

                case "Belleza":
                    linearGradientBrush = new SfLinearGradientBrush
                    {
                        GradientStops = new Syncfusion.XForms.Graphics.GradientStopCollection(){
                    new SfGradientStop(){ Color = Color.FromHex("#7f17bf"), Offset=0.0},
                    new SfGradientStop(){ Color = Color.FromHex("#400c60"), Offset=1.0},
                    }
                    };
                    return linearGradientBrush;

                default:
                    linearGradientBrush = new SfLinearGradientBrush
                    {
                        GradientStops = new Syncfusion.XForms.Graphics.GradientStopCollection(){
                    new SfGradientStop(){ Color = Color.FromHex("#414345"), Offset=0.0},
                    new SfGradientStop(){ Color = Color.FromHex("#232526"), Offset=1.0},
                    }
                    };
                    return linearGradientBrush;
            }
            
        }

    }
}