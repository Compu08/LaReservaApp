using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Firebase.Database;
using Firebase.Database.Query;
using Plugin.LatestVersion;
using Syncfusion.XForms.Graphics;
using TurnosFutbol.DataService;
using TurnosFutbol.Models.Navigation;
using TurnosFutbol.ViewModels.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace TurnosFutbol.Views.Navigation
{
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhotosPage : ContentPage
    {

        public string adminKey;
        public PhotosPage()
        {
            InitializeComponent();
            PopupNavigation.Instance.PushAsync(new Popups.Loading());
            //CheckVersion();
            CityPicker.BindingContext = new CityViewModel();
            PopupNavigation.Instance.PopAsync();
            FillListView();
            UpdateData();
            this.BindingContext = PhotosDataService.Instance.PhotosViewModel;
            SetiOSMargins();
        }

        public async void CheckVersion()
        {
            var isLast = await CrossLatestVersion.Current.IsUsingLatestVersion();
            if (!isLast){
                var alert = await DisplayAlert("Nueva Actualización",
                    "Su versión actual no es la última. Descargá nuestra última actualización para poder continuar usando LaReserva.",
                    "Salir","Actualizar");
                if (!alert)
                    await CrossLatestVersion.Current.OpenAppInStore();
                else
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
            }

        }

        private void SetiOSMargins()
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                TopLayout.Padding = new Thickness(10, 30, 10, 0);
                PrincipalGrid.RowDefinitions.Clear();
                PrincipalGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(85) });
                PrincipalGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(275) });
                PrincipalGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                PrincipalGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }
            else
            {
                TopLayout.Padding = new Thickness(10, 0, 10, 0);
            }

        }

        private void UpdateData()
        {
            adminKey = Preferences.Get("adminKey", "");
            var bossKey = Preferences.Get("bossKey", "");
            if (adminKey != "") adminButton.IsVisible = true;
            if (bossKey != "") bossButton.IsVisible = true;
            if (adminKey == "" && bossKey != "")
            {
                gridAdmins.ColumnDefinitions.Clear();
                gridAdmins.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0) });
                gridAdmins.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            }else if (adminKey != "" && bossKey != "")
            {
                gridAdmins.ColumnDefinitions.Clear();
                gridAdmins.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                gridAdmins.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            }

            labelWelcome.Text = "¡Bienvenido " + Preferences.Get("user", "¡ERROR! NO USER ID") + "!";
        }

        protected override void OnAppearing()
        {
            UpdateData();

            base.OnAppearing();
        }

        private void FillListView()
        {
            List<TypeListView> items = new List<TypeListView>(); ;
            TypeListView futbol = new TypeListView()
            {
                Sport = "Futbol",
                Icon = "\uf1e3",
                BarColor = "#43e815"
            };
            TypeListView tenis = new TypeListView()
            {
                Sport = "Tenis",
                Icon = "\uf45e",
                BarColor = "Orange"
            };
            TypeListView paddle = new TypeListView()
            {
                Sport = "Paddel",
                Icon = "\uf45d",
                BarColor = "#143bff"
            };
            TypeListView squash = new TypeListView()
            {
                Sport = "Squash",
                Icon = "\uf45a",
                BarColor = "Black"
            };
            TypeListView peluqueria = new TypeListView()
            {
                Sport = "Peluqueria",
                Icon = "\uf0c4",
                BarColor = "#fc6fe7"
            };
            TypeListView health = new TypeListView()
            {
                Sport = "Salud",
                Icon = "\uf479",
                BarColor = "#159be8"
            };
            TypeListView laundry = new TypeListView()
            {
                Sport = "Lavadero",
                Icon = "\uf1b9",
                BarColor = "#17bf8a"
            };
            TypeListView beauty = new TypeListView()
            {
                Sport = "Belleza",
                Icon = "\uf5bb",
                BarColor = "#7f17bf"
            };
            TypeListView restaurants = new TypeListView()
            {
                Sport = "Restaurantes",
                Icon = "\uf2e6",
                BarColor = "Red"
            };
            items.Add(futbol);
            items.Add(tenis);
            items.Add(paddle);
            items.Add(squash);
            items.Add(peluqueria);
            items.Add(health);
            items.Add(laundry);
            items.Add(beauty);
            items.Add(restaurants);
            SfListView.ItemsSource = items;
        }

        private async void GoToTurnos(object sender, Syncfusion.ListView.XForms.ItemTappedEventArgs e)
        {
            string sportTitle = ((TypeListView)e.ItemData).Sport.ToString();
            if (LabelPickerCity.Text != "Seleccione una Ciudad")
            {
                await Navigation.PushAsync(new Clubs(LabelPickerCity.Text, sportTitle));
            }
            else
            {
                await DisplayAlert("¡Error!", "Debe seleccionar una ciudad para poder continuar.", "Ok");
            }
        }

        private async void GoToInstructions(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new TurnosFutbol.Views.Onboarding.OnBoardingAnimationPage());
        }
        private async void GoToNotifications(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new TurnosFutbol.Views.Notification.ECommerceNotificationPage());
        }

        private async void GoToAdmin(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new AdminPage());
        }
        private async void GoToBoss(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new BossPage());
        }

        private void City_Tapped(object sender, EventArgs e)
        {
            CityPicker.Focus();
        }

        private void City_Selected(object sender, EventArgs e)
        {
            LabelPickerCity.Text = CityPicker.SelectedItem.ToString();
        }

        private void SearchModal(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Popups.SearchModal());
        }
    }
}