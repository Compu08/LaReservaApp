using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using TurnosFutbol.Models.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TurnosFutbol.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddCity : PopupPage
    {
        public AddCity()
        {
            InitializeComponent();
            crossLabel.Text = "\uf00d";
        }

        private async void AddNewCity(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new Loading());
            FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");
            await fc
                .Child("Ciudades")
                .PostAsync(new City()
                {
                    name = NewCity.Text
                });
            await PopupNavigation.Instance.PopAsync();
            await DisplayAlert("¡Exito!", "Se agregó correctamente " + NewCity.Text + " al listado de Ciudades", "Ok");
            await PopupNavigation.Instance.PopAsync();
        }

        private void CloseModal(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }
    }
}