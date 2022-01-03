using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace TurnosFutbol.Themes
{
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LightTheme
    {
        public LightTheme()
        {
            InitializeComponent();
            SetAccentColor();
        }
        private void SetAccentColor()
        {
            // set Xamarin Color.Accent 
            Xamarin.Forms.Color xamarinAccentColor = Xamarin.Forms.Color.FromHex("#026937"); // Green
            System.Reflection.PropertyInfo accentColorProp = typeof(Xamarin.Forms.Color).GetProperty(nameof(Xamarin.Forms.Color.Accent), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            accentColorProp.SetValue(accentColorProp, xamarinAccentColor, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static, null, null, System.Globalization.CultureInfo.CurrentCulture);
        }
    }
}