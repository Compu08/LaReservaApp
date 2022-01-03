using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Firebase.Database;
using Firebase.Database.Query;
using Syncfusion.XForms.Graphics;
using TurnosFutbol.Models.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;
using GradientStopCollection = Syncfusion.XForms.Graphics.GradientStopCollection;

namespace TurnosFutbol.ViewModels.Navigation
{
    class ShiftsViewModel : BindableObject
    {
        public ObservableCollection<GroupedShifts> shiftsItem;
        public ObservableCollection<Shifts> shiftsTest { get; set; }
        public ObservableCollection<GroupedShifts> ShiftsItem { get { return shiftsItem; } set { shiftsItem = value; OnPropertyChanged(); } }

        public readonly SfLinearGradientBrush styleHeader = new SfLinearGradientBrush();
        public string p;
        public string iconCode;
        public string s;
        private readonly int dayOfTheWeek;
        private Color iColor;
        private Color txtColor;
        private string tT;
        private bool NightMode;
        public bool power;
        public bool countItems;
        public ICommand RefreshList { get; set; }
        public GroupedSpots groupedSpots;

        public ShiftsViewModel(string place, string sport, int d, string textType, bool adminPower, GroupedSpots sL)
        {
            groupedSpots = sL;
            s = sport;
            p = place;
            tT = textType;
            power = adminPower;
            this.shiftsTest = new ObservableCollection<Shifts>();
            NightMode = Preferences.Get("NightMode", false);
            dayOfTheWeek = d;
            ShiftsItem = new ObservableCollection<GroupedShifts>();
            ChangeGradients();
            //RefreshList = new Command(async () => await PerformRefresh());
            if (s == "Gimnasios")
                GetShiftsMultiLevelInformation();
            else
                GetShiftsInformation();
        }

        private static string OutputPhoneChecker(string p, int t, bool adminPower)
        {
            if (!adminPower)
            { 
                return "";
            }

            return t == 2 ? "Esperando Rival" : p;
        }

        private string OutputNameChecker(string n,int t, bool adminPower)
        {
            if (Preferences.Get("user", "") == n)
            {
                return n;
            }
            if (t == 0 && s != "Gimnasios")
            {
                return "DISPONIBLE";
            }
            if (n != "DISPONIBLE" && s == "Gimnasios") return n;

            return adminPower ? n : "OCUPADO";
        }

        private bool ShifFixChecker(int t,string p) {
            if (t == 4 && p != "CLASES")          
                return true;            
            else return false;
        }

        private string getSpotType(int spot) {
            foreach (var item in groupedSpots)
            {
                if (item.number == spot)
                {
                    return item.type;
                }
            }
            return "";
        }
        private int getSpotPrice(int spot) {
            foreach (var item in groupedSpots) {
                if (item.number == spot) { 
                    return item.price;
                }
            }
            return 0;
        }
        private async void GetShiftsInformation()
        {
            FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");
            var GetShifts = (await fc
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
                    outputPhone = item.Object.outputPhone,
                    outputName = item.Object.outputName,
                }).ToList().Where(z=>z.place == p).OrderBy(i => i.hora);

            var headerGroup = GetShifts.Where(e => e.place == p).OrderBy(o => o.spot).Select(x => x.spot).Distinct().ToList();
            countItems = headerGroup.Any();

            foreach (var item in headerGroup)
            {
                var shiftsGroup = new GroupedShifts { spot = tT + " " + groupedSpots[0].number, brushGradient = styleHeader };
                //var shiftsGroup = new GroupedShifts { spot = tT + " " + item, brushGradient = styleHeader };

                var contents = GetShifts.Where(i => i.spot == item).ToList();

                if (contents.Count != 0)
                {
                    foreach (var groupItems in contents)
                    {

                        ColorsAndIcons(groupItems);
                            bool shiftFix = ShifFixChecker(groupItems.tipo,groupItems.persona);
                            var outputPhoneString = OutputPhoneChecker(groupItems.telefono, groupItems.tipo, power);
                            var outputNameString = OutputNameChecker(groupItems.persona,groupItems.tipo,power);
                            var tOS = getSpotType(groupItems.spot);
                            var pOS = getSpotPrice(groupItems.spot);
                            shiftsGroup.Add(new Spots { persona = groupItems.persona, hora = groupItems.hora, telefono = groupItems.telefono, icon = iconCode, textColor = txtColor, iconColor = iColor, id = groupItems.id, tipo = groupItems.tipo, api_id = groupItems.api_id, outputPhone = outputPhoneString, outputName = outputNameString, spot=groupItems.spot, shiftFixed = shiftFix });

                            shiftsTest.Add(new Shifts { persona = groupItems.persona, hora = groupItems.hora, telefono = groupItems.telefono, icon = iconCode, textColor = txtColor, iconColor = iColor, id = groupItems.id, tipo = groupItems.tipo, api_id = groupItems.api_id, outputPhone = outputPhoneString, outputName = outputNameString, spot = groupItems.spot, shiftFixed = shiftFix, typeOfSpot = tOS, priceOfSpot = pOS });
                    }
                    ShiftsItem.Add(shiftsGroup);
                }
            }
        }

        private async void GetShiftsMultiLevelInformation()
        {
            FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");
            var GetShifts = (await fc
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
                    outputPhone = item.Object.outputPhone,
                    outputName = item.Object.outputName,
                    multiShifts = item.Object.multiShifts,
                }).ToList().Where(z => z.place == p).OrderBy(i => i.hora);

            var headerGroup = GetShifts.Where(e => e.place == p).OrderBy(o => o.spot).Select(x => x.spot).Distinct().ToList();
            countItems = headerGroup.Any();

            foreach (var item in headerGroup)
            {
                var shiftsGroup = new GroupedShifts { spot = tT + " " + groupedSpots[0].number, brushGradient = styleHeader };

                var contents = GetShifts.Where(i => i.spot == item).ToList();

                if (contents.Count != 0)
                {
                    foreach (var groupItems in contents)
                    {

                        ColorsAndIcons(groupItems);
                        bool shiftFix = ShifFixChecker(groupItems.tipo, groupItems.persona);
                        var outputPhoneString = OutputPhoneChecker(groupItems.telefono, groupItems.tipo, power);
                        var outputNameString = OutputNameChecker(groupItems.persona, groupItems.tipo, power);
                        var tOS = getSpotType(groupItems.spot);
                        var pOS = getSpotPrice(groupItems.spot);
                        var totalPersons = "Capacidad: 0/" + groupItems.maxShifts.ToString();
                        if (groupItems.multiShifts != null)
                            totalPersons = "Capacidad: "+groupItems.multiShifts.Count.ToString() + "/" + groupItems.maxShifts.ToString();
                            

                        shiftsTest.Add(new Shifts { persona = groupItems.persona, hora = groupItems.hora, telefono = groupItems.telefono, icon = iconCode, textColor = txtColor, iconColor = iColor, id = groupItems.id, tipo = groupItems.tipo, api_id = groupItems.api_id, outputPhone = totalPersons, outputName = outputNameString, spot = groupItems.spot, shiftFixed = shiftFix, typeOfSpot = tOS, priceOfSpot = pOS, multiShifts = groupItems.multiShifts, maxShifts = groupItems.maxShifts});
                    }
                    ShiftsItem.Add(shiftsGroup);
                }
            }
        }

        private void ColorsAndIcons(Shifts g)
        {
            if (s == "Gimnasios") {
                if (g.multiShifts != null) { 
                    foreach (var item in g.multiShifts) {
                        if (item.personaSub == Preferences.Get("user", "NO USER")) {
                            txtColor = Color.FromHex("#007deb");
                            iColor = Color.FromHex("#007deb");
                            iconCode = "\uf058";
                            return;
                        }
                    }
                }
            }
            switch (NightMode)
            {
                case true when (g.tipo == 2):
                    iconCode = "\uf254";
                    txtColor = Color.FromHex("#00ff44");
                    iColor = Color.DeepSkyBlue;
                    break;
                case true when (g.tipo == 0):
                    iconCode = "\uf055";
                    txtColor = Color.FromHex("#00ff44");
                    iColor = txtColor;
                    break;
                case true:
                    iconCode = "\uf057";
                    txtColor = Color.LightGray;
                    iColor = Color.Red;
                    break;
                default:
                {
                    switch (g.tipo)
                    {
                        case 0:
                            iconCode = "\uf055";
                            txtColor = Color.Green;
                            iColor = Color.Green;
                            break;
                        case 2:
                            iconCode = "\uf254";
                            txtColor = Color.Green;
                            iColor = Color.DeepSkyBlue;
                            break;
                        default:
                            iconCode = "\uf057";
                            txtColor = Color.LightGray;
                            iColor = Color.Red;
                            break;
                    }

                    break;
                }
            }
        }

        public void ChangeGradients()
        {
            {
                //SfLinearGradientBrush styleHeader = new SfLinearGradientBrush();
                styleHeader.GradientStops = s switch
                {
                    "Futbol" => new GradientStopCollection
                    {
                        new SfGradientStop {Color = Color.FromHex("#49A800"), Offset = 0.0},
                        new SfGradientStop {Color = Color.FromHex("#3B8205"), Offset = 1.0},
                    },
                    "Tenis" => new GradientStopCollection
                    {
                        new SfGradientStop {Color = Color.FromHex("#F09819"), Offset = 0.0},
                        new SfGradientStop {Color = Color.FromHex("#FF512F"), Offset = 1.0},
                    },
                    "Paddel" => new GradientStopCollection
                    {
                        new SfGradientStop {Color = Color.FromHex("#5691c8"), Offset = 0.0},
                        new SfGradientStop {Color = Color.FromHex("#457fca"), Offset = 1.0},
                    },
                    "Peluqueria" => new GradientStopCollection
                    {
                        new SfGradientStop {Color = Color.FromHex("#f953c6"), Offset = 0.0},
                        new SfGradientStop {Color = Color.FromHex("#b91d73"), Offset = 1.0},
                    },
                    "Restaurantes" => new GradientStopCollection
                    {
                        new SfGradientStop {Color = Color.FromHex("#ED213A"), Offset = 0.0},
                        new SfGradientStop {Color = Color.FromHex("#93291E"), Offset = 1.0},
                    },
                    "Gimnasios" => new GradientStopCollection
                    {
                        new SfGradientStop {Color = Color.FromHex("#414345"), Offset = 0.0},
                        new SfGradientStop {Color = Color.FromHex("#232526"), Offset = 1.0},
                    },
                    _ => styleHeader.GradientStops
                };
            }

        }
    }
}
