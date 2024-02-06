using Rg.Plugins.Popup.Services;
using Syncfusion.XForms.Buttons;
using TurnosFutbol.Interfaces;
using TurnosFutbol.Views.Popups;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using static TurnosFutbol.App;

namespace TurnosFutbol.Views.Settings
{
    /// <summary>
    /// Page to show the setting.
    /// </summary>
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingPage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingPage" /> class.
        /// </summary>
        public SettingPage()
        {
            InitializeComponent();
            NameLabel.Text = Preferences.Get("user", "No user");
            PhoneLabel.Text = Preferences.Get("phone", "Sin Telefono");
            SwitchRecordatorioTurno.IsOn = Preferences.Get("Not_Shift_Recordatory", false);
            SwitchDifusions.IsOn = Preferences.Get("Not_Difusions", false);
            SwitchNightMode.IsOn = Preferences.Get("NightMode", false);
            var city = Preferences.Get("mainCity", "");
            if (city != "")
            {
                CityLabel.Text = city;
            }
        }


        public void Theme_Toggled(object sender, ToggledEventArgs e)
        {
            //bool toggleStatus = switchTheme.IsToggled;
            //SetTheme(toggleStatus);
        }

        private void ChangeName(object sender, System.EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new ChangeName());
        }

        private void ChangePhone(object sender, System.EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new ChangePhone());
        }

        private void ChangeCity(object sender, System.EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new ChangeCity());
        }

        private void SetAdminKey(object sender, System.EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new SetAdminKey());
        }

        private void ChangePaymentLink(object sender, System.EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new ChangePaymentLink());
        }

        private void PushPrivacyPolicy(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new PrivacyPolicy());
        }

        private void PushTermsAndConditions(object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new TermsAndConditions());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            NameLabel.Text = Preferences.Get("user", "No user");
            PhoneLabel.Text = Preferences.Get("phone", "Sin Telefono");
        }

        private void ShiftRecordatoryChanged(object sender, SwitchStateChangedEventArgs e)
        {
            Preferences.Set("Not_Shift_Recordatory", e.NewValue == true);
        }

        private void DifusionsChanged(object sender, SwitchStateChangedEventArgs e)
        {
            Preferences.Set("Not_Difusions", e.NewValue == true);
        }
        private void NightModeChanged(object sender, SwitchStateChangedEventArgs e)
        {
            Preferences.Set("NightMode", e.NewValue == true);
            var toggleStatus = SwitchNightMode.IsOn != null && (bool)SwitchNightMode.IsOn;
            SetTheme(toggleStatus);
        }

        void SetTheme(bool status)
        {
            var themeRequested = status ? Theme.Dark : Theme.Light;

            DependencyService.Get<IAppTheme>().SetAppTheme(themeRequested);
        }

    }
}