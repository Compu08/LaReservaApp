using System;
using System.Collections.Generic;
using System.Text;
using TurnosFutbol.Themes;
using Xamarin.Forms;

[assembly: Dependency(typeof(TurnosFutbol.ThemeHelper))]
namespace TurnosFutbol
{
    
    public class ThemeHelper : IAppTheme
    {
        public void SetAppTheme(App.Theme theme)
        {

            SetTheme(theme);
        }
        void SetTheme(App.Theme mode)
        {
            App.Current.Resources.Clear();
            if (mode == App.Theme.Dark)
            {
                if (App.AppTheme == App.Theme.Dark)
                    return;
                App.Current.Resources = new DarkTheme();
            }
            else
            {
                if (App.AppTheme != App.Theme.Dark)
                    return;
                App.Current.Resources = new LightTheme();
            }
            App.AppTheme = mode;
        }
    }
}
