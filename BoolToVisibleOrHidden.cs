using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace HighlightExplorer
{
	public class BoolToVisibleOrHidden : IValueConverter
	{
		public BoolToVisibleOrHidden()
		{
		}

		// IValueConverter members...
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (bool)value ? Visibility.Visible : Visibility.Hidden;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (Visibility)value == Visibility.Visible;
		}
	}
}
