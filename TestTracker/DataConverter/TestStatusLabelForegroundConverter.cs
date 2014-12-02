using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace TestTracker.DataConverter
{
    public class TestStatusLabelForegroundConverter : IValueConverter
    {
        // Conver TestStatus id to TestStatus String
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string statusText = value.ToString();

            if(statusText.Contains("PASS"))
            {
                return Brushes.LightGreen;
            }
            else if (statusText.Contains("FAIL"))
            {
                return Brushes.Red;
            }
            else if (statusText.Contains("N/A"))
            {
                return Brushes.DimGray;
            }
            else
            {
                return Brushes.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
