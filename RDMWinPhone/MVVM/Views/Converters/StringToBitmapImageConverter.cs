using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace RDMWinPhone
{
    public class StringToBitmapImageConverter : IValueConverter
    {
        #region IValueConverter Membres

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;

            if (value.GetType() != typeof(string) && targetType != typeof(BitmapImage))
            {
                throw new InvalidCastException();
            }

            if (String.IsNullOrWhiteSpace((string)value))
            {
                return null;
            }
            else
            {
                byte[] bytes = System.Convert.FromBase64String((string)value);

                using (InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream())
                {
                    ms.AsStreamForWrite().Write(bytes, 0, bytes.Length);
                    ms.Seek(0);

                    BitmapImage image = new BitmapImage();
                    image.SetSource(ms);
                    return image;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
