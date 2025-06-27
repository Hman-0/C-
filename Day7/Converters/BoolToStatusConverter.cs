using System.Globalization;
using System.Windows.Data;

namespace Day7;

public class BoolToStatusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isCompleted)
        {
            return isCompleted ? "Hoàn thành" : "Chưa hoàn thành";
        }
        return "Chưa hoàn thành";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}