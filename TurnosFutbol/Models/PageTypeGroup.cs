using Syncfusion.XForms.Graphics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace TurnosFutbol.Models
{
    public class PageTypeGroup : List<Turnos>
    {
        public string icon { get; set; }
        public Color iconColor { get; set; }
        public string hora { get; set; }
        public string jugador { get; set; }
        public string telefono { get; set; }
        public Color colorButton { get; set; }

        public int type { get; set; }
        public PageTypeGroup()
        {
        }

        public class GroupedCourts : ObservableCollection<PageTypeGroup>
        {
            public string LongName { get; set; }
            public string ShortName { get; set; }
            public SfLinearGradientBrush brushGradient { get; set; }
        }
    }
}
