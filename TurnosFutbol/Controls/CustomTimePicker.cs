using Syncfusion.SfPicker.XForms;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace TurnosFutbol.Controls
{
    public class CustomTimePicker : SfPicker
    {
        // Time api is used to modify the Hour collection as per change in Time
        /// <summary>
        /// Time is the acutal DataSource for SfPicker control which will holds the collection of Hour ,Minute and Format
        /// </summary>
        public ObservableCollection<object> Time { get; set; }

        //Minute is the collection of minute numbers

        public ObservableCollection<object> Minute;

        //Hour is the collection of hour numbers

        public ObservableCollection<object> Hour;

        //Format is the collection of AM and PM

        public ObservableCollection<object> Places;

        /// <summary>
        /// Header api is holds the column name for every column in time picker
        /// </summary>

        public ObservableCollection<string> Headers { get; set; }
        public CustomTimePicker(string text, int totalPlaces)
        {
            Time = new ObservableCollection<object>();
            Hour = new ObservableCollection<object>();
            Minute = new ObservableCollection<object>();
            Places = new ObservableCollection<object>();
            Headers = new ObservableCollection<string>();
            if (Device.RuntimePlatform == Device.Android)
            {
                Headers.Add("HORA");
                Headers.Add("MINUTOS");
                Headers.Add(text.ToUpper());
            }
            else
            {
                Headers.Add("Hora");
                Headers.Add("Minutos");
                Headers.Add(text);
            }

            //Enable Footer of SfPicker
            ShowFooter = true;

            //Enable Header of SfPicker
            ShowHeader = true;

            //Enable Column Header of SfPicker
            ShowColumnHeader = true;


            //SfPicker header text
            HeaderText = "SELECCIONAR HORA Y LUGAR";

            PopulateTimeCollection(totalPlaces);
            this.ItemsSource = Time;

            // Column header text collection
            this.ColumnHeaderText = Headers;
        }

        private void PopulateTimeCollection(int totalPlaces)
        {
            //Populate Hour
            for (int i = 0; i <= 23; i++)
            {
                Hour.Add(i.ToString("00"));
            }

            //Populate Minute
            for (int j = 0; j < 60; j++)
            {
                if (j < 10)
                {
                    Minute.Add("0" + j);
                }
                else
                {
                    Minute.Add(j.ToString());
                }
            }

            for (int i = 0; i <= totalPlaces; i++)
            {
                Places.Add((i + 1).ToString());
            }

            Time.Add(Hour);
            Time.Add(Minute);
            Time.Add(Places);
        }




    }
}
