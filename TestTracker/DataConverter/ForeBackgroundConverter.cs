using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using TestTracker.Core.Utils;

namespace TestTracker.DataConverter
{
    public class ForeBackgroundConverter : IValueConverter
    {
        // Strip off the time part of the dateTime
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int testStatusId = int.Parse(value.ToString());
            if(testStatusId == (int)EnumTestStatus.Completed)
            {
                return Brushes.LightGreen;
            }
            else if (testStatusId == (int)EnumTestStatus.Pending)
            {
                return Brushes.DimGray;
            }
            else if (testStatusId == (int)EnumTestStatus.Processing || testStatusId == (int)EnumTestStatus.Running)
            {
                return Brushes.Yellow;
            }
            else if (testStatusId == (int)EnumTestStatus.Stopped || testStatusId == (int)EnumTestStatus.Uncompleted
                || testStatusId == (int)EnumTestStatus.FailConnection || testStatusId == (int)EnumTestStatus.BusyConnection
                || testStatusId == (int)EnumTestStatus.WrongHBAConfig)
            {
                return Brushes.Red;
            }
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
