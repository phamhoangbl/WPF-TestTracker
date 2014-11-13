using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TestTracker.DataConverter
{
    public class ShortScriptNameConverter : IValueConverter
    {
        // Strip off the time part of the dateTime
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string filePath = value.ToString();
            return filePath.Substring(filePath.LastIndexOf("\\", filePath.Length - 1) + 1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
