using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text.RegularExpressions;

namespace TurnosFutbol.Models.Navigation
{
    public class ClubsList
    {
        public string place { get; set; }
        public string picture { get; set; }
        public string adress { get; set; }
        public string value { get; set; }
        public string courts { get; set; }
        public string city { get; set; }
        public string phone { get; set; }
        public string type { get; set; }
        public string price { get; set; }
        public bool payment { get; set; }
        public bool hide { get; set; }
        public string paymentT { get; set; }
        public Color paymentC { get; set; }
        public string hideT { get; set; }
        public Color hideC { get; set; }
        public string seller { get; set; }
        public int id { get; set; }
        public bool vip { get; set; }
        public string contacto { get; set; }
        public TimeSpan timebefore { get; set; }
        public string password { get; set; }
        public bool isLocked { get; set; }
        public GroupedSpots spotList { get; set; }

    }

    public class GroupedSpots : ObservableCollection<OneSpot>
    {
        public int n { get; set; }
    }

    public class OneSpot
    {
        public int price { get; set; }
        public string type { get; set; }
        public int number { get; set; }
        public string spotString { get; set; }
        public string priceString { get; set; }
    }

    public class PlacesForPass
    {
        public string place { get; set; }
    }

}
