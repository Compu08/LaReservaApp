using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TurnosFutbol.Models.Navigation;
using Xamarin.Forms;

namespace TurnosFutbol.ViewModels.Navigation
{
    class ClubsViewModel : BindableObject
    {
        public ObservableCollection<ClubsList> clubsItem;
        public ObservableCollection<ClubsList> ClubsItem { get { return clubsItem; } set { clubsItem = value; OnPropertyChanged(); } }
        private string t;
        private string c;
        private readonly FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");

        public ICommand RefreshList { get; set; }
        public string tT;
        public ClubsViewModel(string type, string city, string textType)
        {
            t = type;
            c = city;
            tT = textType;
            ClubsItem = new ObservableCollection<ClubsList>();
            RefreshList = new Command(async () => await PerformRefresh());
            GetClubsInformation();

        }
        private async Task PerformRefresh()
        {
            ClubsItem = new ObservableCollection<ClubsList>();
            GetClubsInformation();
        }
        private string CapitalizeText(string str)
        {
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        private string getValue(string v)
        {
            if (!string.IsNullOrEmpty(v))
            {
                if (t == "Gimnasios" || t == "Peluqueria")
                    return "Valor: $" + v;
                else
                    return "Turno: $" + v;
            }
            else
            {
                return "";
            }
        }

        private string getCourts(string c)
        {
            if (t == "Gimnasios")
                return "";
            else
                return CapitalizeText(tT) + ": " + c;
        }

        private async void GetClubsInformation()
        {
            var getNews = (await fc
                .Child("Clients")
                .OnceAsync<ClubsList>()).Select(item => new ClubsList
                {
                    id = item.Object.id,
                    place = item.Object.place,
                    picture = item.Object.picture,
                    adress = item.Object.adress,
                    value = getValue(item.Object.value),
                    courts = getCourts(item.Object.courts),
                    city = item.Object.city,
                    phone = item.Object.phone,
                    hide = item.Object.hide,
                    type = item.Object.type,
                    vip = item.Object.vip,
                    timebefore = item.Object.timebefore,
                    spotList = item.Object.spotList,
                    isLocked = item.Object.isLocked,
                    password = item.Object.password,
                }).Where(a => a.type == t).ToList().OrderBy(i => i.place);

            var vipPlaces = getNews.Where(i => i.vip == true);

            foreach (ClubsList item in getNews)
            {
                if ((item.city == c) && (item.hide == false))
                {
                    ClubsItem.Add(item);
                }
            }

            ClubsItem = new ObservableCollection<ClubsList>(ClubsItem.OrderBy(x => Guid.NewGuid()).ToList());
            vipPlaces = vipPlaces.OrderBy(x => Guid.NewGuid()).ToList();

            foreach (ClubsList item in vipPlaces)
            {
                ClubsItem.Remove(item);
                ClubsItem.Insert(0, item);
            }
        }

    }
}

