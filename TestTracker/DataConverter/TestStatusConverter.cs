using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TestTracker.Core.Utils;

namespace TestTracker.DataConverter
{
    public class TestStatusConverter : IValueConverter
    {
        // Conver TestStatus id to TestStatus String
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int testStatusId = int.Parse(value.ToString());
            string stringValue = Enum.GetName(typeof(EnumTestStatus), testStatusId);

            string empty = stringValue;
            return empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
