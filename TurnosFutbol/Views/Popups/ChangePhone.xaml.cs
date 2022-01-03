using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;

namespace TurnosFutbol.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChangePhone : PopupPage
    {
        public ChangePhone()
        {
            InitializeComponent();
            NewPhone.Text = Preferences.Get("phone", "Sin Telefono");
            CrossLabel.Text = "\uf00d";
        }

        private void SaveNewPhone(object sender, EventArgs e)
        {
            Preferences.Set("phone", NewPhone.Text.ToString());
            PopupNavigation.Instance.PopAsync();
        }

        private void CloseModal(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }
    }
}