using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TurnosFutbol.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotificationPopup : PopupPage
    {
        public NotificationPopup(string title, string description, string time,string place, string logo)
        {
            InitializeComponent();
            CrossLabel.Text = "\uf00d";

            TitleLabel.Text = title;
            PlaceLabel.Text = "por " + place;
            TextLabel.Text = description;
            LogoLabel.Source = logo;
            TimeLabel.Text = time;
        }

        private void CloseModal(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }

    }
}