using TurnosFutbol.Interfaces;
using TurnosFutbol.Themes;
using Xamarin.Forms;
using static TurnosFutbol.App;

[assembly: Dependency(typeof(TurnosFutbol.Droid.ThemeHelper))]
namespace TurnosFutbol.Droid
{
    public class ThemeHelper : IAppTheme
    {
        public void SetAppTheme(App.Theme theme)
        {
            SetTheme(theme);
        }

        void SetTheme(Theme mode)
        {
            if (mode == Theme.Dark)
            {
                if (App.AppTheme == Theme.Dark)
                {
                    return;
                }
                App.Current.Resources = new DarkTheme();
            }
            else
            {
                if (App.AppTheme != Theme.Dark)
                {
                    return;
                }
                App.Current.Resources = new LightTheme();
            }
            App.AppTheme = mode;
        }
    }
}