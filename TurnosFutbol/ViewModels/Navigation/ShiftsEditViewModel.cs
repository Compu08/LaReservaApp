using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TurnosFutbol.Models.Navigation;
using Xamarin.Forms;

namespace TurnosFutbol.ViewModels.Navigation
{
    class ShiftsEditViewModel : BindableObject
    {
        public ObservableCollection<GroupedShifts> shiftsItem;
        public ObservableCollection<GroupedShifts> ShiftsItem { get { return shiftsItem; } set { shiftsItem = value; OnPropertyChanged(); } }

        public string p;
        public string iconCode;
        public string s;
        public int dayOfTheWeek;
        public Color iColor;
        public Color txtColor;
        public ICommand RefreshList { get; set; }

        public ShiftsEditViewModel(string place, string sport, int d)
        {
            s = sport;
            p = place;
            dayOfTheWeek = d;
            ShiftsItem = new ObservableCollection<GroupedShifts>();
            RefreshList = new Command(async () => await PerformRefresh());
            GetShiftsInformation();
        }

        private async Task PerformRefresh()
        {
            ShiftsItem = new ObservableCollection<GroupedShifts>();
            GetShiftsInformation();
        }
        private async void GetShiftsInformation()
        {
            FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");
            IOrderedEnumerable<Shifts> GetShifts = (await fc
                .Child("Turnos")
                .Child(dayOfTheWeek.ToString())
                .Child(s)
                .OnceAsync<Shifts>()).Select(item => new Shifts
                {
                    persona = item.Object.persona,
                    hora = item.Object.hora,
                    place = item.Object.place,
                    spot = item.Object.spot,
                    telefono = item.Object.telefono,
                    tipo = item.Object.tipo,
                    maxShifts = item.Object.maxShifts,
                    classType = item.Object.classType,
                    api_id = item.Object.api_id,
                    id = item.Object.id,
                }).ToList().OrderBy(i => i.hora);

            List<int> headerGroup = GetShifts.OrderBy(o => o.spot).Select(x => x.spot).Distinct().ToList();

            foreach (int item in headerGroup)
            {
                GroupedShifts shiftsGroup = new GroupedShifts() { spot = "CANCHA " + item };

                List<Shifts> contents = GetShifts.Where(i => i.spot == item).ToList();

                if (contents.Count != 0)
                {
                    foreach (Shifts groupItems in contents)
                    {
                        if (groupItems.place == p)
                        {
                            if (groupItems.tipo == 0)
                            {
                                iconCode = "\uf055";
                                txtColor = Color.Green;
                                iColor = Color.Green;
                            }
                            else
                            {
                                iconCode = "\uf057";
                                txtColor = Color.LightGray;
                                iColor = Color.Red;
                            }
                            shiftsGroup.Add(new Spots() { persona = groupItems.persona, hora = groupItems.hora, telefono = groupItems.telefono, icon = iconCode, textColor = txtColor, iconColor = iColor, id = groupItems.id });
                        }
                    }
                    ShiftsItem.Add(shiftsGroup);
                }
            }

        }

    }
}
