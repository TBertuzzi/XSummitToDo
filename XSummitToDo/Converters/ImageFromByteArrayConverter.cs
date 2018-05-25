using System;
using System.IO;
using Xamarin.Forms;

namespace XSummitToDo.Converters
{
    public class ImageFromByteArrayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            byte[] bytes = (byte[])value;
            Stream stream = new MemoryStream(bytes);

            return ImageSource.FromStream(() => stream);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
