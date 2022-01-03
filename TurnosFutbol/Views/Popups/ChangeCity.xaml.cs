using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using TurnosFutbol.ViewModels.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TurnosFutbol.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChangeCity : PopupPage
    {
        public ChangeCity()
        {
            InitializeComponent();
            crossLabel.Text = "\uf00d";
            CitysConstructor();
        }

        private void CitysConstructor()
        {
            PopupNavigation.Instance.PushAsync(new Popups.Loading());
            CityPicker.BindingContext = new CityViewModel();
            PopupNavigation.Instance.PopAsync();
        }

        private void CloseModal(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }
        private async void SaveNewCity(object sender, EventArgs e)
        {
            Preferences.Set("mainCity",CityPicker.SelectedItem.ToString());
            await DisplayAlert("¡Éxito!",
                "Se asignó correctamente " + CityPicker.SelectedItem.ToString() + " como sú ciudad","Ok");
            await PopupNavigation.Instance.PopAsync();
        }
    }
}