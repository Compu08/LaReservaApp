using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using Xamarin.Forms.Xaml;

namespace TurnosFutbol.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdminPowerNoShift : PopupPage
    {
        public AdminPowerNoShift(int shiftId, int type)
        {
            InitializeComponent();
        }


        private void CloseModal(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }

        private async void Delete_Shift(object sender, EventArgs e)
        {

        }

    }
}