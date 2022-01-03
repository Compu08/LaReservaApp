using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Firebase.Database;
using Prism.Mvvm;
using TurnosFutbol.Models.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TurnosFutbol.ViewModels.Navigation
{
    class NotificationsViewModel : BindableObject
    {
        public ObservableCollection<Models.Navigation.Notification> notificationsList;
        public ObservableCollection<Models.Navigation.Notification> NotificationsList { get => notificationsList;
            set { notificationsList = value; OnPropertyChanged(); } }

        public int itemCount;

        public int theItemCount { get => itemCount; set { itemCount = value;OnPropertyChanged(); } }

        private readonly FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");

        public NotificationsViewModel()
        {
            NotificationsList = new ObservableCollection<Models.Navigation.Notification>();
            GetNotifications();
        }

        private async void GetNotifications()
        {
            var myUser = Preferences.Get("user", "");
            var myPhone = Preferences.Get("phone", "");
            var getNotifications = (await fc
                .Child("Notificaciones")
                .OnceAsync<Models.Navigation.Notification>()).Select(item => new Models.Navigation.Notification
                {
                    TimeDif = item.Object.Time.Day+"/"+item.Object.Time.Month+"/"+item.Object.Time.Year+" - "+item.Object.Time.ToString("HH:mm")+" hs.",
                    //TimeDif = theItemCount.ToString(),
                    Description = item.Object.Description,
                    Icon = getIcon(item.Object.Title),
                    IconColor = getIconColor(item.Object.Title),
                    User = item.Object.User,
                    Token = item.Object.Token,
                    Topic = item.Object.Topic,
                    Id = item.Object.Id,
                    Title = item.Object.Title,
                    Phone = item.Object.Phone,
                    Place = item.Object.Place,
                    Time = item.Object.Time,
                }).ToList();

            var orderedList = getNotifications.Where(i=>i.Time > DateTime.Now.AddDays(-10)).OrderByDescending(x => x.Time);

            foreach (var item in orderedList)
            {
                NotificationsList.Add(item);
                theItemCount++;
            }

            theItemCount = NotificationsList.Count;

        }

        public string getIcon(string title)
        {
            var icon = title switch
            {
                "OFERTA" => "\uf02c",
                "Difusión Turnos" => "\uf017",
                "Información Torneo" => "\uf091",
                "Nueva Noticia" => "\uf1ea",
                _ => ""
            };

            return icon;
        }

        public Color getIconColor(string title)
        {
            var iconC = title switch
            {
                "OFERTA" => Color.Orange,
                "Difusión Turnos" => Color.DeepSkyBlue,
                "Información Torneo" => Color.Red,
                "Nueva Noticia" => Color.ForestGreen,
                _ => Color.Blue
            };

            return iconC;
        }
    }
}
