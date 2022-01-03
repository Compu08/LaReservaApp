using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;

namespace TurnosFutbol.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Loading : PopupPage
    {
        float progressMax = 10;
        float progress = 0;
        int counter = 1;
        bool isTimerRunning = true;
        public Loading()
        {
            InitializeComponent();
            //   TimerCloser();
        }

        private void CloseModal(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }

        private async void TimerCloser()
        {
            await Task.Delay(3000);
            await Navigation.PopPopupAsync();
        }
    }
}