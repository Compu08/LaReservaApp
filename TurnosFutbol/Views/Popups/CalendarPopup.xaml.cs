using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TurnosFutbol.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalendarPopup : PopupPage
    {
        public event EventHandler<DateTime> CallbackEvent;
        // or Callback

        public CalendarPopup(DateTime selectedD)
        {
            InitializeComponent();
            CrossLabel.Text = "\uf00d";
            newDatePicker.SelectedDate = selectedD;
            newDatePicker.MaxDate = DateTime.Now.AddDays(5);
            newDatePicker.MinDate = DateTime.Now;
            Month.Text = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(newDatePicker.SelectedDate.Value.Month).ToUpper() + " " + newDatePicker.SelectedDate.Value.Year.ToString().ToUpper();
            Day.Text = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(newDatePicker.SelectedDate.Value.DayOfWeek).ToUpper();
            newDatePicker.MonthViewSettings.TodaySelectionBackgroundColor = Color.FromHex("#19478F");
            newDatePicker.MonthViewSettings.TodayBorderColor = Color.FromHex("#4acaff");
            newDatePicker.MonthViewSettings.DateSelectionColor = Color.FromHex("#19478F");
            newDatePicker.MonthViewSettings.DayHeaderTextColor = Color.Black;
            newDatePicker.MonthViewSettings.DayHeaderFontAttributes = FontAttributes.Bold;
            newDatePicker.MonthViewSettings.DayHeaderFontSize = 14;

        }
        private void CloseModal(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }

        private void InvoceCallback()
        {
            CallbackEvent?.Invoke(this, newDatePicker.SelectedDate.Value);
        }

        protected override void OnDisappearing() => CallbackEvent?.Invoke(this, newDatePicker.SelectedDate.Value);

        private void NewUpdateLabelDate(object sender, Syncfusion.SfCalendar.XForms.SelectionChangedEventArgs e)
        {
            Day.Text = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(newDatePicker.SelectedDate.Value.DayOfWeek).ToUpper();
            PopupNavigation.Instance.PopAsync();
        }

        private void UpdateMonthLabel(object sender, Syncfusion.SfCalendar.XForms.MonthChangedEventArgs e)
        {
            Month.Text = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(newDatePicker.SelectedDate.Value.Month).ToUpper() + " " + newDatePicker.SelectedDate.Value.Year.ToString().ToUpper();
        }
    }
}