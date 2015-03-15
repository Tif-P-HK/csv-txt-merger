using System;
using System.Windows;
using System.Windows.Data;

namespace Simulation.Converters
{
  public class CountToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return (int)value == 0 ? Visibility.Hidden : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class CountToEnabledConverter : IValueConverter
  {
     public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return (int)value >= 2 ? true : false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
