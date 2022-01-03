using Syncfusion.XForms.Graphics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace TurnosFutbol.Models.Navigation
{
    public class Shifts
    {
        public string hora { get; set; }
        public string persona { get; set; }
        public string telefono { get; set; }
        public string place { get; set; }
        public int id { get; set; }
        public int tipo { get; set; }
        public int spot { get; set; }
        public int maxShifts { get; set; }
        public string classType { get; set; }
        public string api_id { get; set; }
        public string icon { get; set; }
        public Color textColor { get; set; }
        public Color iconColor { get; set; }
        public string outputPhone { get; set; }
        public string outputName { get; set; }
        public bool shiftFixed { get; set; }
        public string typeOfSpot { get; set; }
        public int priceOfSpot { get; set; }
        public SubShifts multiShifts { get; set; }
        public string personaSub { get; set; }
        public int itemIndex { get; set; }
    }

    public class Spots : List<Shifts>
    {
        public string hora { get; set; }
        public string persona { get; set; }
        public string telefono { get; set; }
        public string place { get; set; }
        public int id { get; set; }
        public int tipo { get; set; }
        public int spot { get; set; }
        public int maxShifts { get; set; }
        public string classType { get; set; }
        public string api_id { get; set; }
        public string icon { get; set; }
        public Color textColor { get; set; }
        public Color iconColor { get; set; }
        public string outputPhone { get; set; }
        public string outputName { get; set; }
        public bool shiftFixed { get; set; }
        public string typeOfSpot { get; set; }
        public int priceOfSpot { get; set; }
        public SubShifts multiShifts { get; set; }
        public string personaSub { get; set; }
        public int itemIndex { get; set; }
    }

    public class SubShifts : List<SubShift> { 
    
    }
    public class SubShift {
        public int itemIndex { get; set; }
        public string personaSub { get; set; }
        public string telefonoSub { get; set; }
        public bool fixedSub { get; set; }
        public int idSub { get; set; }
    }
    public class GroupedShifts : ObservableCollection<Spots>
    {
        public string spot { get; set; }
        public SfLinearGradientBrush brushGradient { get; set; }

    }


}


