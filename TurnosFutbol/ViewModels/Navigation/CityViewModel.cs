using Firebase.Database;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TurnosFutbol.Models.Navigation;
using Xamarin.Forms;

namespace TurnosFutbol.ViewModels.Navigation
{
    class CityViewModel : BindableObject
    {
        private ObservableCollection<string> citysItem;
        public ObservableCollection<string> CitysItem { get { return citysItem; } set { citysItem = value; OnPropertyChanged(); } }
        public ICommand RefreshList { get; set; }

        public CityViewModel()
        {
            CitysItem = new ObservableCollection<string>();
            GetCitysInformation();
        }

        private async Task PerformRefresh()
        {
            CitysItem = new ObservableCollection<string>();
            GetCitysInformation();
        }

        private async void GetCitysInformation()
        {
            FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");
            var GetCitys = (await fc
                .Child("Ciudades")
                .OnceAsync<City>()).Select(item => new City
                {
                    name = item.Object.name,
                }).ToList().OrderBy(i => i.name);

            foreach (City item in GetCitys)
            {
                CitysItem.Add(item.name);
            }
        }
    }


}
