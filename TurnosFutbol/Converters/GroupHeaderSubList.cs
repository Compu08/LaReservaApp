using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TurnosFutbol.Models.Navigation;
using Xamarin.Forms;

namespace TurnosFutbol.Converters
{
    public class GroupHeaderSubList : IValueConverter
    {
        public string p;
        public GroupHeaderSubList(string param) {
            p = param;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var obj = value as string;
            if (p == "hora")
                return obj;



            return "MILE NDEA";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
