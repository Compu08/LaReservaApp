using Firebase.Database;
using Firebase.Database.Query;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Plugin.FirebasePushNotification;
using Syncfusion.DataSource;
using Syncfusion.DataSource.Extensions;
using Syncfusion.ListView.XForms;
using Syncfusion.XForms.Graphics;
using TurnosFutbol.Models.Navigation;
using TurnosFutbol.ViewModels.Navigation;
using TurnosFutbol.Views.Popups;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ItemTappedEventArgs = Syncfusion.ListView.XForms.ItemTappedEventArgs;
using System.Collections;
using Xamarin.Forms.OpenWhatsApp;
using Plugin.Messaging;
using TurnosFutbol.Converters;

namespace TurnosFutbol.Views.Navigation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShiftsPage : ContentPage
    {
        public bool allowSwitch;
        public string s;
        public string p;
        public int dayOfTheWeek;
        public string selectedDay;
        public string userData;
        public string phoneData;
        public bool userPower;
        public string userApiId;
        public string placePower;
        public string shiftByAdmin;
        public int selectedShift;
        public bool notRecordatory;
        public string textForAlert;
        public Shifts obj;
        public int holdedShift;
        public string adminTopic;
        public TimeSpan timeB;
        public FirebaseClient fc = new FirebaseClient("https://lareserva-9d2f1-default-rtdb.firebaseio.com/");
        private string textType;
        public SfLinearGradientBrush gradient;
        public GroupedSpots spotsList;
        public DateTime dateSelected;
        public ShiftsPage(string place, string sport, DateTime d,TimeSpan timeBefore, SfLinearGradientBrush b, GroupedSpots spotL)
        {
            InitializeComponent();
            titleName.Text = place;
            spotsList = spotL;
            timeB = timeBefore;
            s = sport;
            p = place;
            dateSelected = d;
            adminTopic = p.Replace(" ", ".");
            gradient = b;
            userPower = false;
            LabelEmpty.IsVisible = false;
            allowSwitch = true;
            PopupNavigation.Instance.PushAsync(new Loading());
            CheckAdminPowers(p);
            PickerShiftAdminConstructor();
            TextTypeConstructor();
            DateConstructors(d);
            PopupNavigation.Instance.PopAsync();
        }
        private void GoBack(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void DateConstructors(DateTime d)
        {
            dayOfTheWeek = (int)d.DayOfWeek;
            datePicker.MaximumDate = DateTime.Now.AddDays(5);
            datePicker.MinimumDate = DateTime.Now;
            datePicker.Date = d;
            userData = Preferences.Get("user", "¡ERROR! NO USER");
            phoneData = Preferences.Get("phone", "Sin Teléfono");
            notRecordatory = Preferences.Get("Not_Shift_Recordatory", false);
            userApiId = Preferences.Get("api_id", "¡ERROR! NO API ID");
            var strDay = CapitalizeText($"{datePicker.Date:dddd}");
            selectedDay = strDay + " " + datePicker.Date.Day + "/" + datePicker.Date.Month;
            LabelDate.Text = Convert.ToString(new CultureInfo("en-US", false).TextInfo.ToTitleCase(String.Format("{0:dddd d 'de' MMMM}", datePicker.Date))).ToUpper();
        }

        private string CapitalizeText(string str)
        {
            return char.ToUpper(str[0]) + str.Substring(1);
        }
        private void TextTypeConstructor()
        {
            switch (s)
            {
                case "Futbol":
                    textType = "cancha";
                    textForAlert = "al equipo de";
                    break;
                case "Tenis":
                    textType = "cancha";
                    textForAlert = "a";
                    break;
                case "Paddel":
                    textType = "cancha";
                    textForAlert = "a";
                    break;
                case "Restaurantes":
                    textType = "mesa";
                    break;
                case "Peluqueria":
                    textType = "silla";
                    break;
                case "Gimnasios":
                    textType = "clases";
                    break;
                default:
                    textType = "turno";
                    break;
            }
        }


        private async void CheckAdminPowers(string placeString)
        {
            if (Preferences.Get("adminKey", "NO KEY") == "NO KEY") return;

            var keyCheck = (await fc
                .Child("Admins")
                .Child(Preferences.Get("adminKey", "NO KEY"))
                .Child(s)
                .OnceAsync<Admins>()).Where(a => a.Object.place == p).ToList();

            foreach (var item in keyCheck.Where(item => item.Object.place == p))
            {
                userPower = true;
                lstView.AllowSwiping = true;
                var topicToSubscribe = item.Object.place.Replace(" ", ".");
                CrossFirebasePushNotification.Current.Subscribe(topicToSubscribe);
            }

            //And then put the shifts in the list
            RefreshShifts(p, s, dayOfTheWeek, textType.ToUpper(), userPower, spotsList);
            
        }

        private void RefreshShifts(string place, string sport, int d, string text, bool admin, GroupedSpots sL)
        {
            lstView.BindingContext = new ShiftsViewModel(place, sport, d, text, admin, sL);
            GroupHeaderConstructor(gradient);
        }

        public void GroupHeaderConstructor(SfLinearGradientBrush b)
        {
            if (lstView.DataSource.GroupDescriptors.Count > 0) return;
            lstView.GroupHeaderTemplate = new DataTemplate(() =>
            {
                var grid = new Grid
                {
                    RowSpacing = 0,
                    RowDefinitions =
                    { 
                        new RowDefinition{Height = 30},
                        new RowDefinition{Height = GridLength.Star},

                    },
                    ColumnDefinitions =
                    {
                        new ColumnDefinition{Width = GridLength.Auto},
                        new ColumnDefinition{Width = GridLength.Star},
                        new ColumnDefinition{Width = GridLength.Auto},
                        new ColumnDefinition{Width = GridLength.Star},
                    },
                    Padding = new Thickness(0,5,0,0),
                    BackgroundColor = (Color)Application.Current.Resources["BackgroundBase"],
                };
                var linearGradientBrush = new SfLinearGradientBrush
                {
                    GradientStops = new Syncfusion.XForms.Graphics.GradientStopCollection(){
                    new SfGradientStop(){ Color = Color.FromHex("#2c4065"), Offset=0.0},
                    new SfGradientStop(){ Color = Color.FromHex("#234263"), Offset=1.0},
                    }
                };
                var gradient = new SfGradientView
                {
                    BackgroundBrush = b,
                };
                var label = new Label
                {
                    FontSize = 15,
                    FontAttributes = FontAttributes.Bold,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start,
                    Margin = new Thickness(20, 0, 0, 0),
                    Padding = new Thickness(0, 5, 0, 5),
                    TextColor = Color.White,
                };
                var iconLabel = new Label
                {
                    Text = "\uf107",
                    FontSize = 18,
                    TextColor = Color.White,
                    HorizontalOptions = LayoutOptions.End,
                    Margin = new Thickness(0, 0, 20, 0),
                    Padding = new Thickness(0, 2, 0, 2),
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    FontFamily = (OnPlatform<string>)Application.Current.Resources["FontIcons-2"],
                };
                var labelType = new Label
                {
                    FontSize = 12,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Start,
                    Margin = new Thickness(20, 0, 0, 0),
                    Padding = new Thickness(0,2, 0, 2),
                    TextColor = Color.White,
                };
                labelType.SetBinding(Label.TextProperty, new Binding("Key",BindingMode.Default, new GroupHeaderConverter(1)));
                

                var labelPrice = new Label
                {
                    Text = "",
                    FontSize = 12,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.End,
                    Margin = new Thickness(0, 0, 20, 0),
                    Padding = new Thickness(0, 2, 0, 2),
                    TextColor = Color.White,
                };
                labelPrice.SetBinding(Label.TextProperty, new Binding("Key", BindingMode.Default, new GroupHeaderConverter(2)));

                var labelBackground = new Label {
                    BackgroundColor = Color.FromHex("#2c4065"),
                };
                label.SetBinding(Label.TextProperty, new Binding("Key",BindingMode.Default, new GroupHeaderConverter(0)));

                grid.Children.Add(gradient);
                Grid.SetRow(gradient, 0);
                Grid.SetColumnSpan(gradient, 4);
                grid.Children.Add(label, 0, 0);
                grid.Children.Add(iconLabel, 3, 0);

                if (s == "Futbol" || s == "Tenis" || s == "Paddel" || s == "Peluqueria")
                {

                    grid.Children.Add(labelBackground, 0, 1);
                    grid.Children.Add(labelPrice, 3, 1);
                    grid.Children.Add(labelType, 0, 1);
                    Grid.SetRow(labelPrice, 1);
                    Grid.SetRow(labelType, 1);
                    Grid.SetRow(labelBackground, 1);
                    Grid.SetColumnSpan(labelBackground, 4);
                }
                else
                    lstView.GroupHeaderSize = 32;

                return grid;

            });
            
            lstView.DataSource.AutoExpandGroups = true;
            lstView.DataSource.GroupDescriptors.Add(new GroupDescriptor()
            {
                PropertyName = "spot",
                KeySelector = (object obj1) =>
                {
                    var item = (obj1 as Shifts);
                    if (s == "Gimnasios")
                        return textType.ToUpper() + " "+"/"+"*";
                    else 
                        return textType.ToUpper() + " " + item.spot +"/"+ item.typeOfSpot+"*"+item.priceOfSpot;
                }
            });

        }

        public class GroupHeaderConverter : IValueConverter
        {
            int t;
            public GroupHeaderConverter(int type) {
                t = type;
            }
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                var str = (string)value;
                if (t == 0)
                    return str.Substring(0,str.IndexOf("/"));
                if (t == 1)
                    return str.Substring(str.IndexOf("/") + "/".Length, str.IndexOf("*") - (str.IndexOf("/") + "/".Length));
                if (t == 2) { 
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

        private void PickerShiftAdminConstructor()
        {
            ObservableCollection<string> items = new ObservableCollection<string>();
            if (s == "Futbol" || s == "Tenis" || s == "Paddel")
            {
                items.Add("Reservar Turno");
                items.Add("Clases");
                items.Add("Torneo");
            }
            else
            {
                items.Add("Reservar Turno");
            }
            ShiftAdminPicker.ItemsSource = items;
        }

        private async void PickedShiftOptionAdmin(object sender, EventArgs e)
        {
            string selectedOption = ShiftAdminPicker.SelectedItem as string;
            AdminPowerShift page = new AdminPowerShift(selectedShift, s, dayOfTheWeek, selectedDay, obj.hora);
            ShiftAdminPicker.SelectedIndex = -1;
            switch (selectedOption)
            {
                case "Reservar Turno":
                    await PopupNavigation.Instance.PushAsync(page);
                    page.Disappearing += (sender2, e2) =>
                    {
                        RefreshShifts(p, s, dayOfTheWeek, textType.ToUpper(), userPower, spotsList);
                    };

                    break;
                case "Torneo":
                    shiftByAdmin = "TORNEO";
                    await PopupNavigation.Instance.PushAsync(new Popups.Loading());
                    await UpdateShift(selectedShift, 4);
                    await PopupNavigation.Instance.PopAsync();
                    await DisplayAlert("¡Exito!", "Turno Reservado para el día " + selectedDay + " a las " + obj.hora + " hs.", "Ok");
                    RefreshShifts(p, s, dayOfTheWeek, textType.ToUpper(), userPower, spotsList);
                    break;
                case "Clases":
                    shiftByAdmin = "CLASES";
                    await PopupNavigation.Instance.PushAsync(new Popups.Loading());
                    await UpdateShift(selectedShift, 4);
                    await PopupNavigation.Instance.PopAsync();
                    await DisplayAlert("¡Exito!", "Turno Reservado para el día " + selectedDay + " a las " + obj.hora + " hs.", "Ok");
                    RefreshShifts(p, s, dayOfTheWeek, textType.ToUpper(), userPower, spotsList);
                    break;
            }
        }

        ///DATABASE FUNCTION FOR UPDATE
        public async Task UpdateShift(int pickedId, int reservationType)
        {
            var topicToSubscribe = p.Replace(" ", "_");
            CrossFirebasePushNotification.Current.Subscribe(topicToSubscribe);

            var toUpdateShift = (await fc
                .Child("Turnos")
                .Child(dayOfTheWeek.ToString())
                .Child(s)
                .OnceAsync<Shifts>()).ToList();

            var shiftObject = toUpdateShift.First(a => a.Object.id == pickedId);
            string person = userData;
            string phone = phoneData;
            int ShiftType = 1;
            string id_api = userApiId;

            switch (reservationType)
            {
                case 0: //Shift Reserved
                    break;

                case 1: //Shift Removed
                    person = "DISPONIBLE";
                    phone = "";
                    ShiftType = 0;
                    id_api = "";
                    break;

                case 2: //Shift Waiting for Rival
                    person = userData;
                    id_api = userApiId;
                    ShiftType = 2;
                    break;

                case 3: //Shift Waiting accepted
                    person = shiftObject.Object.persona;
                    phone = shiftObject.Object.telefono;
                    id_api = shiftObject.Object.api_id;
                    ShiftType = 1;
                    break;

                case 4: //Shift By TheAdmin
                    person = shiftByAdmin;
                    phone = "";
                    id_api = "";
                    ShiftType = 1;
                    break;
                case 5: //Shift Permanent
                    person = shiftObject.Object.persona;
                    phone = shiftObject.Object.telefono;
                    id_api = userApiId;
                    ShiftType = 4;
                    break;

            }

            if (s != "Gimnasios")
            {
                await fc.Child("Turnos").Child(dayOfTheWeek.ToString()).Child(s).Child(shiftObject.Key).PutAsync(new Models.Navigation.Shifts()
                {
                    persona = person,
                    telefono = phone,
                    tipo = ShiftType,
                    api_id = id_api,
                    place = shiftObject.Object.place,
                    spot = shiftObject.Object.spot,
                    maxShifts = shiftObject.Object.maxShifts,
                    id = shiftObject.Object.id,
                    hora = shiftObject.Object.hora,
                    shiftFixed = shiftObject.Object.shiftFixed,
                });
            }
            else {
                int multiShiftsCount = 0;
                if (shiftObject.Object.multiShifts != null)
                    multiShiftsCount = shiftObject.Object.multiShifts.Count;

                if (shiftObject.Object.maxShifts != multiShiftsCount)
                {
                    var rndm = new Random();
                    var subId = rndm.Next(0, 99999);

                    var newMulti = new SubShifts();
                    if(shiftObject.Object.multiShifts != null)
                        newMulti = shiftObject.Object.multiShifts;

                    int newMultiCount = 0;
                    if (newMulti != null)
                        newMultiCount = newMulti.Count;

                    newMulti.Add(new SubShift { personaSub = person, telefonoSub = phone, idSub = subId, itemIndex = (newMultiCount+1) });

                    var type = shiftObject.Object.tipo;
                    var shiftTitle = shiftObject.Object.persona;

                    if ( newMultiCount == shiftObject.Object.maxShifts) { 
                        shiftTitle = "OCUPADO";
                        type = 1;
                    }

                    await fc.Child("Turnos").Child(dayOfTheWeek.ToString()).Child(s).Child(shiftObject.Key).PutAsync(new Shifts()
                    {
                        persona = shiftTitle,
                        telefono = shiftObject.Object.telefono,
                        tipo = type,
                        api_id = shiftObject.Object.api_id,
                        place = shiftObject.Object.place,
                        spot = shiftObject.Object.spot,
                        maxShifts = shiftObject.Object.maxShifts,
                        id = shiftObject.Object.id,
                        hora = shiftObject.Object.hora,
                        multiShifts = newMulti,
                    });
                }
                else {
                    await DisplayAlert("¡Error!", "El turno ya se encuentra en su capacidad máxima.", "Ok");
                }
            }
        }
        private async void CancelInMultiShift(int idShift, string person) {
            await PopupNavigation.Instance.PushAsync(new Loading());

            var toDelete = (await fc
                .Child("Turnos")
                .Child(dayOfTheWeek.ToString())
                .Child(s)
                .OnceAsync<Shifts>()).FirstOrDefault(a => a.Object.id == idShift);

            var multiShiftsNewList = new SubShifts();

            if (toDelete.Object.multiShifts != null)
                multiShiftsNewList = toDelete.Object.multiShifts;

            foreach (var item in multiShiftsNewList.ToList()) {
                if (item.personaSub == person)
                    multiShiftsNewList.Remove(item);
            }

            await fc
                .Child("Turnos")
                .Child(dayOfTheWeek.ToString())
                .Child(s)
                .Child(toDelete.Key)
                .PutAsync(new Shifts()
                {
                    persona = "DISPONIBLE",
                    api_id = toDelete.Object.api_id,
                    hora = toDelete.Object.hora,
                    iconColor = toDelete.Object.iconColor,
                    id = toDelete.Object.id,
                    maxShifts = toDelete.Object.maxShifts,
                    place = toDelete.Object.place,
                    spot = toDelete.Object.spot,
                    telefono = toDelete.Object.telefono,
                    textColor = toDelete.Object.textColor,
                    tipo = 0,
                    shiftFixed = toDelete.Object.shiftFixed,
                    multiShifts = multiShiftsNewList,
                });

            RefreshShifts(p, s, dayOfTheWeek, textType.ToUpper(), userPower, spotsList);

            await PopupNavigation.Instance.PopAsync();
        }

        private TimeSpan CalculateTime()
        {
            var now = DateTime.Now;
            var pickedHour = Convert.ToInt32((obj.hora).Substring(0, 2));
            DateTime pickedDate;
            if (Device.RuntimePlatform == "iOS")
            {
                pickedDate = new DateTime(dateSelected.Date.Year, dateSelected.Date.Month, dateSelected.Date.Day, pickedHour, 0, 0);
            }
            else {
                pickedDate = new DateTime(datePicker.Date.Year, datePicker.Date.Month, datePicker.Date.Day, pickedHour, 0, 0);
            }
            
            var finalDate = pickedDate - now;
            return finalDate;
        }

        ///Reserve Shift Logic
        private async void ShiftPicked(object sender, ItemTappedEventArgs e)
        {
            bool hasShift = false;
            string text = "";
            if (e.ItemType == ItemType.GroupHeader) return;

            obj = e.ItemData as Shifts;

            if (s == "Gimnasios" && userPower)
            {
                    MultiShiftsList page = new MultiShiftsList(obj.multiShifts, selectedDay, obj.hora, dayOfTheWeek, s, obj.id);
                    await PopupNavigation.Instance.PushAsync(page);

                    page.Disappearing += (sender2, e2) =>
                    {
                       RefreshShifts(p, s, dayOfTheWeek, textType.ToUpper(), userPower, spotsList);
                    };

                    return;
            }

            selectedShift = obj.id;
            ///THE ADMIN IS HERE!!! Or maybe not..
            if (s == "Gimnasios")
            {
                if (obj.multiShifts != null) { 
                    foreach (var item in obj.multiShifts)
                    {
                        if (item.personaSub == userData)
                            hasShift = true;
                    }
                }
            }
            else {
                text = " en " + textType.ToUpper() + " " + obj.spot;
            }
            if (obj.tipo == 1 && s == "Gimnasios" && hasShift || obj.tipo == 0 && s == "Gimnasios" && hasShift)
            {
                bool answerCancel = await DisplayAlert("Eliminar Turno", "¿Quieres cancelar tú turno?", "Si", "No");
                if (answerCancel) { 
                    CancelInMultiShift(obj.id, userData);
                    Notification("Reserva Cancelada", selectedDay + " - " + obj.hora + " cancelado por " + userData+" en " + p, "/topics/" + adminTopic);
                    await DisplayAlert("¡Exito!", "Tu turno del día " + selectedDay + " a las " + obj.hora + " hs. fué cancelado", "Ok");
                }
            }
            else if (obj.persona == "DISPONIBLE" && obj.tipo == 0)
            {
                if (userPower && s != "Gimnasios")
                {
                    ///Admin Options
                    ShiftAdminPicker.Focus();
                }
                else if (timeB < CalculateTime())
                {
                    ///THE USER WANTS TO MAKE A RESERVATION
                    var answerReservation = await DisplayAlert("Reservar Turno", "¿Desea guardar el turno de las " + obj.hora + " hs?", "Si", "No");
                    if (answerReservation)
                    {
                        if (s == "Futbol" || s == "Tenis" || s == "Paddel")
                        {
                            var answer2 = await DisplayAlert("Reservar Turno", "¿Quiere anotarse para esperar rival?", "Si",
                            "No");
                            if (answer2)
                            {
                                await PopupNavigation.Instance.PushAsync(new Popups.Loading());
                                await UpdateShift(Convert.ToInt32(obj.id), 2);
                                RefreshShifts(p, s, dayOfTheWeek, textType.ToUpper(), userPower, spotsList);
                                Notification("Nueva Reserva - Esperando Rival", selectedDay + " - " + obj.hora + " hs. reservado por " + userData + text + " en " + p, "/topics/" + adminTopic);
                                await DisplayAlert("¡Exito!", "Estas esperando Rival para el día " + selectedDay + " a las " + obj.hora + " hs.", "Ok");
                                await PopupNavigation.Instance.PopAsync();
                            }
                            else
                            {
                                await PopupNavigation.Instance.PushAsync(new Popups.Loading());
                                await UpdateShift(Convert.ToInt32(obj.id), 0);
                                RefreshShifts(p, s, dayOfTheWeek, textType.ToUpper(), userPower, spotsList);
                                Notification("Nueva Reserva", selectedDay + " - " + obj.hora + " hs. reservado por " + userData + text + " en " + p, "/topics/" + adminTopic);
                                await DisplayAlert("¡Exito!", "Turno Reservado para el día " + selectedDay + " a las " + obj.hora + " hs.", "Ok");
                                await PopupNavigation.Instance.PopAsync();
                            }
                        }
                        else
                        {
                            await PopupNavigation.Instance.PushAsync(new Popups.Loading());
                            await UpdateShift(Convert.ToInt32(obj.id), 0);
                            RefreshShifts(p, s, dayOfTheWeek, textType.ToUpper(), userPower, spotsList);
                            Notification("Nueva Reserva", selectedDay + " - " + obj.hora + " hs. reservado por " + userData + text + " en " + p, "/topics/" + adminTopic);
                            await DisplayAlert("¡Exito!", "Turno Reservado para el día " + selectedDay + " a las " + obj.hora + " hs.", "Ok");
                            await PopupNavigation.Instance.PopAsync();
                        }

                    }
                    else
                    {
                        //lstView.SelectedItems.Clear();
                    }
                }
                else
                {
                    var time = CalculateTime();
                    if (time.Hours < 0 || time.Minutes < 0) await DisplayAlert("¡Error!",
                        "Este turno no puede ser tomado ya que es anterior a la hora actual", "Ok");
                    else
                        await DisplayAlert("¡Error!",
                            "El turno debe ser sacado con " + timeB.Hours + " horas de anticipación", "Ok");
                }
            }

            else if (obj.tipo == 2 && userData == obj.persona || obj.tipo == 1 && userData == obj.persona)
            {
                ///THE USER WANTS TO CANCEL THE RESERVATION
                bool answerCancel = await DisplayAlert("Eliminar Turno", "¿Quieres cancelar tú turno?", "Si", "No");
                if (answerCancel)
                {
                    await PopupNavigation.Instance.PushAsync(new Popups.Loading());
                    await UpdateShift(Convert.ToInt32(obj.id), 1);
                    RefreshShifts(p, s, dayOfTheWeek, textType.ToUpper(), userPower, spotsList);
                    await PopupNavigation.Instance.PopAsync();
                    Notification("Reserva Cancelada", selectedDay + " - " + obj.hora + " cancelado por " + userData + text + p, "/topics/" + adminTopic);
                    await DisplayAlert("¡Exito!", "Tu turno del día " + selectedDay + " a las " + obj.hora + " hs. fué cancelado", "Ok");
                    ///Push notification to Admins Place AND Cancel Local Notification
                }
            }
            else if (obj.tipo == 2)
            {
                ///THE USER WANTS TO ACCEPT A CHALLENGE

                bool answerVersus = await DisplayAlert("Reservar Turno", "¿Desea enfrentar " + textForAlert + " " + obj.persona + " en el turno de las " + obj.hora + " hs?", "Si", "No");
                if (answerVersus)
                {
                    ///Push notification to rival AND push notification to admins
                    await PopupNavigation.Instance.PushAsync(new Popups.Loading());
                    await UpdateShift(Convert.ToInt32(obj.id), 3);
                    RefreshShifts(p, s, dayOfTheWeek, textType.ToUpper(), userPower, spotsList);
                    await PopupNavigation.Instance.PopAsync();
                    Notification("Encuentro Confirmado", selectedDay + " - " + obj.hora + " pactado por " + userData + "con " + obj.persona + " en " + textType.ToUpper() + " " + obj.spot + " en " + p, "/topics/" + adminTopic);
                    Notification("Encuentro Confirmado", selectedDay + " - " + obj.hora + "hs. pactado por " + userData + " en " + textType.ToUpper() + " " + obj.spot + " en " + p, "/topics/"+obj.api_id);
                    if (phoneData != "Sin Teléfono")
                    {
                        await DisplayAlert("¡Exito!", "Encuentro pactado para el día " + selectedDay + " a las " + obj.hora + " hs. El teléfono de tu rival es " + obj.telefono, "Ok"); ;
                    }
                    else
                    {
                        await DisplayAlert("¡Exito!", "Encuentro pactado para el día " + selectedDay + " a las " + obj.hora + " hs. Tú rival no proporcionó su numero de teléfono para contacto.", "Ok"); ;
                    }
                }
            }
            else
            {
                if (userPower)
                {
                    bool answerCancel = await DisplayAlert("Eliminar Turno", "¿Quieres cancelar el turno de las " + obj.hora + " hs.?", "Si", "No");
                    if (answerCancel)
                    {
                        ///Admin wants to delete a reservation
                        await PopupNavigation.Instance.PushAsync(new Popups.Loading());
                        await UpdateShift(Convert.ToInt32(obj.id), 1);
                        RefreshShifts(p, s, dayOfTheWeek, textType.ToUpper(), userPower, spotsList);
                        await PopupNavigation.Instance.PopAsync();
                        await DisplayAlert("¡Exito!", "El turno del día " + selectedDay + " a las " + obj.hora + " hs. fué cancelado", "Ok");
                    }
                }
                else
                {
                    ///User wants to make a reservation in already reserved court
                    await DisplayAlert("¡Error!", "Esta " + textType + " se encuentra ocupada", "Entendido");
                }
            }

        }

        private void LocalNotification()
        {
            if (notRecordatory)
            {
                ///Put the Local Notification
            }
        }
        private void UpdateLabelDate(object sender, DateChangedEventArgs e)
        {
            //await PopupNavigation.Instance.PushAsync(new Popups.Loading());
            dayOfTheWeek = (int) e.NewDate.DayOfWeek;
            var strDay = CapitalizeText($"{datePicker.Date:dddd}");
            selectedDay = strDay + " " + datePicker.Date.Day + "/" + datePicker.Date.Month;
            LabelDate.Text = Convert.ToString(new CultureInfo("en-US", false).TextInfo.ToTitleCase(String.Format("{0:dddd d 'de' MMMM}", datePicker.Date))).ToUpper();
            lstView.IsVisible = true;
            RefreshShifts(p, s, dayOfTheWeek, textType.ToUpper(), userPower, spotsList);
            //await PopupNavigation.Instance.PopAsync();
        }

        private void OpenDate(object sender, EventArgs e)
        {
            if (Device.RuntimePlatform == "iOS")
            {
                var popupPage = new CalendarPopup(dateSelected);
                popupPage.CallbackEvent += (object sender, DateTime e) => CallbackMethod(sender, e);
                PopupNavigation.Instance.PushAsync(popupPage);
            }
            else
            {
                datePicker.Focus();
            }
        }

        private void CallbackMethod(object sender, DateTime e) {
            dateSelected = e;
            dayOfTheWeek = (int)e.DayOfWeek;
            var strDay = CapitalizeText($"{e.Date:dddd}");
            selectedDay = strDay + " " + e.Date.Day + "/" + e.Date.Month;
            LabelDate.Text = Convert.ToString(new CultureInfo("en-US", false).TextInfo.ToTitleCase(String.Format("{0:dddd d 'de' MMMM}", e.Date))).ToUpper();
            RefreshShifts(p, s, dayOfTheWeek, textType.ToUpper(), userPower, spotsList);
        }

        private void Notification(string theTitle, string text, string t)
        {
            FCMNotification notif = new FCMNotification();
            
            //var tNew = t.Replace(" ", "_");
            notif.SendNotification(theTitle, text, t);
            //AndroidNotif fcmPush = new AndroidNotif();
            //fcmPush.SendNotification(theTitle, text, tNew);
        }

        private void DisplayLabelEmpty()
        {
            LabelEmpty.IsVisible = true;
            lstView.IsVisible = false;
        }


        private async void ShiftFixed(object sender, ItemHoldingEventArgs e)
        {
            obj = e.ItemData as Shifts;
            if (obj.tipo == 0 && s != "Gimnasios" || obj.tipo == 2 && s != "Gimnasios" || userPower == false) return;
            if (userPower && s != "Gimnasios")
            {
                if (obj.tipo == 4)
                {
                    bool exitPermanent = await DisplayAlert("Tipo de Turno", "¿Quieres que el turno de las " + obj.hora + " hs. ya no sea fijo?", "Si", "No");
                    if (exitPermanent)
                    {
                        ///Admin wants to not make the shift permanent
                        await PopupNavigation.Instance.PushAsync(new Popups.Loading());
                        await UpdateShift(Convert.ToInt32(obj.id), 1);
                        RefreshShifts(p, s, dayOfTheWeek, textType.ToUpper(), userPower, spotsList);
                        await PopupNavigation.Instance.PopAsync();

                        await DisplayAlert("¡Exito!", "El turno del día " + selectedDay + " a las " + obj.hora + " hs. ya no es fijo", "Ok");
                    }
                }
                else
                {
                    bool answerPermanent = await DisplayAlert("Tipo de Turno", "¿Quieres fijar permanentemente el turno de las " + obj.hora + " hs.?", "Si", "No");
                    if (answerPermanent)
                    {
                        ///Admin wants to make the shift permanent
                        await PopupNavigation.Instance.PushAsync(new Popups.Loading());
                        await UpdateShift(Convert.ToInt32(obj.id), 5);
                        RefreshShifts(p, s, dayOfTheWeek, textType.ToUpper(), userPower, spotsList);
                        await PopupNavigation.Instance.PopAsync();

                        Notification("Turno fijado", selectedDay + " - " + obj.hora + " fijado por " + userData + " en " + textType.ToUpper() + " " + obj.spot, "/topics/" + adminTopic);
                        await DisplayAlert("¡Exito!", "El turno del día " + selectedDay + " a las " + obj.hora + " hs. fué fijado", "Ok");
                    }
                }
            }
            else if (userPower && s == "Gimnasios") {
                holdedShift = obj.id;
                GymClassPicker.Focus();
            }
        }

        private void lstView_SwipeEnded(object sender, Syncfusion.ListView.XForms.SwipeEndedEventArgs e)
        {
            var selectedObj = e.ItemData as Shifts;
            if (allowSwitch && selectedObj.tipo != 0 && selectedObj.persona != "CLASES" && selectedObj.persona != "TORNEO" && selectedObj.persona.ToLower() != "escuelita" && selectedObj.persona.ToLower() != "peña")
            {
                if (e.SwipeDirection == Syncfusion.ListView.XForms.SwipeDirection.Right)
                    Chat.Open("54" + selectedObj.telefono, "");
                else
                {
                    // CrossMessaging.Current.PhoneDialer.MakePhoneCall("+54" + selectedObj.telefono);

                    var alias = Preferences.Get("paymentLink", "");

                    var text = "¡Hola! Te escribimos de " + titleName.Text + " para solicitarte la seña del turno que reservaste, de lo contrario será cancelado a la brevedad." + Environment.NewLine + "Por favor adjuntar comprobante de pago." + Environment.NewLine + Environment.NewLine + alias;

                    Chat.Open("54" + selectedObj.telefono, text);
                }
            }

            this.lstView.ResetSwipe();
        }

        private void lstView_SwipeStarted(object sender, Syncfusion.ListView.XForms.SwipeStartedEventArgs e)
        {
            try
            {
                if ((e.ItemData as Shifts).telefono.Length > 0)
                    return;
            }
            catch {
                allowSwitch = false;
                DisplayAlert("¡Error!", "El Cliente no brindó su número telefónico.", "Ok");
            }
        }

        private async void GymClassPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedOption = GymClassPicker.SelectedItem as string;
            switch (selectedOption) {
                case "Body Combat":
                case "Body Pump":
                case "Localizado":
                case "Pilates":
                case "Spinning":
                case "Yoga":
                case "XCore":
                case "Zumba":
                    //Update the Shift to new class type
                    await PopupNavigation.Instance.PushAsync(new Loading());
                    var toUpdateShift = (await fc
                    .Child("Turnos")
                    .Child(dayOfTheWeek.ToString())
                    .Child(s)
                    .OnceAsync<Shifts>()).ToList();

                    var shiftObject = toUpdateShift.First(a => a.Object.id == holdedShift);

                    await fc.Child("Turnos").Child(dayOfTheWeek.ToString()).Child(s).Child(shiftObject.Key).PutAsync(new Models.Navigation.Shifts()
                    {
                        persona = selectedOption.ToUpper(),
                        telefono = shiftObject.Object.telefono,
                        tipo = shiftObject.Object.tipo,
                        api_id = shiftObject.Object.api_id,
                        place = shiftObject.Object.place,
                        spot = shiftObject.Object.spot,
                        maxShifts = shiftObject.Object.maxShifts,
                        id = shiftObject.Object.id,
                        hora = shiftObject.Object.hora,
                        shiftFixed = shiftObject.Object.shiftFixed,
                    });
                    await DisplayAlert("¡Exito!", "Se cambió el tipo de clase de manera correcta.", "Ok");
                    RefreshShifts(p, s, dayOfTheWeek, textType.ToUpper(), userPower, spotsList);
                    await PopupNavigation.Instance.PopAsync();
                    break;

                case "Otro":
                    OtherClassType page = new OtherClassType(holdedShift, dayOfTheWeek.ToString(), s);
                    await PopupNavigation.Instance.PushAsync(page);
                    page.Disappearing += (sender2, e2) =>
                    {
                        RefreshShifts(p, s, dayOfTheWeek, textType.ToUpper(), userPower, spotsList);
                    };
                    break;
            }
        }

    }
}




