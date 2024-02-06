using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Globalization;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;

namespace TurnosFutbol.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChangePaymentLink : PopupPage
    {
        public ChangePaymentLink()
        {
            InitializeComponent();
            NewName.Text = Preferences.Get("paymentLink", "");
            CrossLabel.Text = "\uf00d";
        }

        private void SaveNewName(object sender, EventArgs e)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            var strUser = textInfo.ToTitleCase(NewName.Text.ToLower());
            Preferences.Set("paymentLink", strUser);
            PopupNavigation.Instance.PopAsync();
        }

        private void CloseModal(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }

    }
}