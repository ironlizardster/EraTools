using System;
using System.IO;
using System.Windows.Data;

namespace EraCsvManager.Converter
{
    public class FolderPathIsPresentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return Directory.Exists(value.ToString());
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
