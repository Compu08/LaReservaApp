using Firebase.Database;
using Firebase.Database.Query;
using Plugin.FirebasePushNotification;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;

namespace TurnosFutbol.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetAdminKey : PopupPage
    {
        private FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");
        public SetAdminKey()
        {
            InitializeComponent();
            AdminKey.Text = Preferences.Get("adminKey", "");
            CrossLabel.Text = "\uf00d";
        }
        private void CloseModal(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }
        public void CheckKey(object sender, EventArgs e)
        {
            CheckKeyOnline();
        }

        public async void CheckKeyOnline()
        {
            await PopupNavigation.Instance.PushAsync(new Loading());
            if (AdminKey.Text == "BOSS-f8hh9ccm")
            {
                Preferences.Set("bossKey", AdminKey.Text);
                await DisplayAlert("¡Exito!", "Tu clave es correcta y se te otorgaron privilegios de administrador.", "Ok");
                await PopupNavigation.Instance.PopAllAsync();
            }
            else
            {
                var toCheck = (await fc
                    .Child("Admins")
                    .OnceAsync<Models.Navigation.Admins>()).Where(a => a.Key == AdminKey.Text).ToList();

                if (toCheck.Count > 0)
                {
                    await PopupNavigation.Instance.PopAsync();
                    CrossFirebasePushNotification.Current.UnsubscribeAll();
                    Preferences.Set("adminKey", AdminKey.Text);
    
                    var toCheck2 = (await fc
                    .Child("Admins")
                    .Child(AdminKey.Text)
                    .OnceAsync<Models.Navigation.Admins>()).Select(item => new Models.Navigation.Admins { place = item.Key }).ToList();

                    foreach (var item in toCheck2) {
                        {
                            var query = (await fc
                             .Child("Admins")
                             .Child(AdminKey.Text)
                             .Child(item.place)
                                .OnceAsync<Models.Navigation.Admins>()).Select(item => new Models.Navigation.Admins
                                {
                                    place = item.Object.place,
                                }).ToList();

                            foreach (var thePlace in query)
                            {
                                var topicToSubscribe = thePlace.place.Replace(" ", ".");
                                CrossFirebasePushNotification.Current.Subscribe(topicToSubscribe);
                                CrossFirebasePushNotification.Current.Subscribe("admins");
                            }
                        }

                        }
                    await DisplayAlert("¡Exito!", "Tu clave es correcta y se te otorgaron privilegios de administrador.", "Ok");
                    await PopupNavigation.Instance.PopAsync();
                }
                else
                {
                    await PopupNavigation.Instance.PopAsync();
                    await DisplayAlert("¡Error!", "La Clave ingresada no existe en el sistema.", "Ok");
                    await PopupNavigation.Instance.PopAsync();
                }
            }
        }

    }
}