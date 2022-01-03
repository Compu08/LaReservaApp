using Firebase.Database;
using Firebase.Database.Query;
using Plugin.Segmented.Control;
using Plugin.Segmented.Event;
using Rg.Plugins.Popup.Services;
using Syncfusion.DataSource;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TurnosFutbol.Models.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TurnosFutbol.Views.Navigation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShiftsEditor : ContentPage
    {
        public FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");
        public Shifts obj;
        public int selectedDay;
        public ObservableCollection<string> types;
        public ObservableCollection<string> places;
        private int placeSpots;
        private string stringTypeSpot;
        private string spotType;
        public GroupedSpots sL;
        public int selectedPlaceId;
        public ShiftsEditor()
        {
            InitializeComponent();
            selectedDay = 0;
            places = new ObservableCollection<string>();
            types = new ObservableCollection<string>();
            sL = new GroupedSpots();
            TypePickerConstructor();
        }

        private void GoBack(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void SpotTypeConstructor()
        {
            stringTypeSpot = typePicker.SelectedItem.ToString() switch
            {
                "Futbol" => "CANCHA",
                "Tenis" => "CANCHA",
                "Paddel" => "CANCHA",
                "Restaurantes" => "MESA",
                "Peluqueria" => "SILLA",
                "Gimnasios" => "CLASES",
                _ => stringTypeSpot
            };
        }

        private async void DeleteItem(object sender, Syncfusion.ListView.XForms.ItemTappedEventArgs e)
        {
            obj = e.ItemData as Shifts;
            var answer = obj != null && await DisplayAlert("Eliminar Turno", "¿Desea eliminar el turno de " + obj.hora + " hs. ?", "Si", "No");
            if (answer)
            {
                await DeleteShift(obj.id);
            }
        }

        public async Task DeleteShift(int shiftId)
        {
            await PopupNavigation.Instance.PushAsync(new Popups.Loading());
            var toDeleteShift = (await fc
                .Child("Turnos")
                .Child(selectedDay.ToString())
                .Child(typePicker.SelectedItem.ToString())
                .OnceAsync<Models.Navigation.Shifts>()).FirstOrDefault(a => a.Object.id == shiftId);

            await fc.Child("Turnos").Child(selectedDay.ToString()).Child(typePicker.SelectedItem.ToString()).Child(toDeleteShift.Key).DeleteAsync();

            await DisplayAlert("¡Exito!", "Se eliminó el turno de las " + obj.hora, "Ok");

            ReloadShifts(placePicker.SelectedItem.ToString(), typePicker.SelectedItem.ToString(), selectedDay);
            await PopupNavigation.Instance.PopAllAsync();
        }

        private void DayChanged(object sender, SegmentSelectEventArgs e)
        {
            selectedDay = ((SegmentedControl)sender).SelectedSegment;
            if (placePicker.SelectedIndex != -1 && typePicker.SelectedIndex != -1)
            {
                ReloadShifts(placePicker.SelectedItem.ToString(), typePicker.SelectedItem.ToString(), selectedDay);
            }
        }
        private void GroupHeaderConstructor(string s) {
            if (lstView.DataSource.GroupDescriptors.Count > 0) return;

            lstView.GroupHeaderTemplate = new DataTemplate(() =>
            {
                var grid = new Grid {
                    BackgroundColor = (Color)Application.Current.Resources["ButtonBackground"],
                    RowSpacing = 0,
                    RowDefinitions =
                    {
                        new RowDefinition{Height = 30},
                        new RowDefinition{Height = GridLength.Star},

                    },
                    ColumnDefinitions =
                    {
                        new ColumnDefinition{Width = GridLength.Star},
                        new ColumnDefinition{Width = GridLength.Star},
                    },
                };
                var labelSpot = new Label {
                    FontSize = 14,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    TextColor = Color.White,
                    Margin = new Thickness(8,0,0,0),
                };
                labelSpot.SetBinding(Label.TextProperty, new Binding("Key", BindingMode.Default, new GroupHeaderConverter(0)));

                var labelType = new Label
                {
                    FontSize = 12,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    TextColor = Color.White,
                    Margin = new Thickness(8, 0, 0, 0),
                };
                labelType.SetBinding(Label.TextProperty, new Binding("Key", BindingMode.Default, new GroupHeaderConverter(1)));

                var labelPrice = new Label
                {
                    FontSize = 12,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                    TextColor = Color.White,
                    Margin = new Thickness(0, 0, 8, 0),
                };
                labelPrice.SetBinding(Label.TextProperty, new Binding("Key", BindingMode.Default, new GroupHeaderConverter(2)));

                var labelIcon = new Label
                {
                    Text = "\uf107",
                    FontSize = 18,
                    TextColor = Color.White,
                    HorizontalOptions = LayoutOptions.End,
                    Margin = new Thickness(0, 0, 8, 0),
                    VerticalOptions = LayoutOptions.Center,
                    FontFamily = (OnPlatform<string>)Application.Current.Resources["FontIcons-2"],
                };

                var backgroundRow = new StackLayout
                {
                    BackgroundColor = Color.FromHex("#4EC3E9"),
                    Padding = new Thickness(0,3,0,3)
                };


                grid.Children.Add(labelSpot, 0, 0);
                grid.Children.Add(labelIcon, 1, 0);

                    grid.Children.Add(backgroundRow, 0, 1);
                    Grid.SetColumnSpan(backgroundRow, 2);
                    grid.Children.Add(labelType, 0, 1);
                    grid.Children.Add(labelPrice, 1, 1);

                return grid;
            } );

            lstView.DataSource.AutoExpandGroups = true;
            lstView.DataSource.GroupDescriptors.Add(new GroupDescriptor()
            {
                PropertyName = "spot",
                KeySelector = (object obj1) =>
                {
                    var item = (obj1 as Shifts);
                    if(s == "Gimnasios")
                        return stringTypeSpot.ToUpper() + "/"+" "+"*"+" ";
                    else
                        return stringTypeSpot.ToUpper() + " " + item.spot + "/" + item.typeOfSpot + "*" + item.priceOfSpot;
                }
            });

        }

        public class GroupHeaderConverter : IValueConverter
        {
            int t;
            public GroupHeaderConverter(int type)
            {
                t = type;
            }
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                var str = (string)value;
                if (t == 0)
                    return str.Substring(0, str.IndexOf("/"));
                if (t == 1)
                    return str.Substring(str.IndexOf("/") + "/".Length, str.IndexOf("*") - (str.IndexOf("/") + "/".Length));

                if (t == 2)
                {
                    var substr = str.Substring(str.LastIndexOf('*') + 1);
                    if (substr != "0")
                        return "$" + substr;
                    else
                        return "";
                }

                    return null;

            }
            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

            public void ReloadShifts(string place, string sport, int day)
        {
            PopupNavigation.Instance.PushAsync(new Popups.Loading());
            GetGroupedSpots(place,sport,day);
            PopupNavigation.Instance.PopAllAsync();
        }

        private async void AddNewShift(object sender, EventArgs e)
        {
            var typeSpot = (await fc
                .Child("Clients")
                .OnceAsync<ClubsList>())
                .Select(item => new ClubsList()
                {
                    courts = item.Object.courts,
                    place = item.Object.place,
                    type = item.Object.type,
                }).OrderBy(i => i.place);

            foreach (var item in typeSpot)
            {
                if (item.place == placePicker.SelectedItem.ToString())
                {
                    if (item.type == "Gimnasios")
                    {
                        spotType = "CAPACIDAD";
                        placeSpots = 99;
                    }
                    else {
                        spotType = stringTypeSpot;
                        placeSpots = Convert.ToInt32(item.courts);
                    }
                }
            }
            var page = new Popups.AddNewShift(selectedDay, typePicker.SelectedItem.ToString(), placePicker.SelectedItem.ToString(), placeSpots, spotType);
            page.Disappearing += (sender2, e2) => { ReloadShifts(placePicker.SelectedItem.ToString(), typePicker.SelectedItem.ToString(), selectedDay); };
            await PopupNavigation.Instance.PushAsync(page);
        }
        public async void TypePickerConstructor()
        {
            var typePickerDB = (await fc
                .Child("Admins")
                .Child(Preferences.Get("adminKey",""))
                .OnceAsync<Admins>()).Select(item => new Admins
                {
                    place = item.Key
                }).ToList().OrderBy(i => i.place);

            if (typePickerDB.Count() != 0)
            {
                foreach (Admins item in typePickerDB)
                {
                    Admins placeName = new Admins() { place = item.place };
                    types.Add(placeName.place.ToString());
                }
            }
            typePicker.ItemsSource = types;
            typePicker.SelectedIndex = 0;
            PlacePickerConstructor(typePicker.SelectedItem.ToString());
        }
        public async void PlacePickerConstructor(string type)
        {
            var typePickerDB = (await fc
                .Child("Admins")
                .Child(Preferences.Get("adminKey",""))
                .Child(type.ToString())
                .OnceAsync<Admins>()).Select(item => new Admins
                {
                    place = item.Object.place,
                }).ToList().OrderBy(i => i.place);

            if (typePickerDB.Count() != 0)
            {
                places.Clear();
                foreach (Admins item in typePickerDB)
                {
                    Admins placeName = new Admins() { place = item.place };
                    places.Add(placeName.place.ToString());
                }
            }

            placePicker.ItemsSource = places;
            placePicker.SelectedIndex = 0;
            SpotTypeConstructor();
        }

        private async void GetGroupedSpots(string place, string sport, int day) {
            GroupHeaderConstructor(sport);
            var getSpots = (await fc
                           .Child("Clients")
                           .OnceAsync<ClubsList>()).Select(item => new ClubsList
                           {
                               place = item.Object.place,
                               spotList = item.Object.spotList,
                           }).Where(a => a.place == placePicker.SelectedItem.ToString()).ToList().FirstOrDefault();

            sL = getSpots.spotList;

            lstView.BindingContext = new ViewModels.Navigation.ShiftsViewModel(place, sport, day, stringTypeSpot, true, sL);
        }

        private async void TypePicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new Popups.Loading());
            PlacePickerConstructor(typePicker.SelectedItem.ToString());
            await PopupNavigation.Instance.PopAllAsync();
        }

        private void PlacePicked(object sender, EventArgs e)
        {
            if (typePicker.SelectedIndex != -1 && placePicker.SelectedIndex != -1)
            {
                addButton.IsVisible = true;
                ReloadShifts(placePicker.SelectedItem.ToString(), typePicker.SelectedItem.ToString(), selectedDay);
            }
        }

    }
}