using Hami.WPF.IDETool.Common.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Hami.WPF.IDETool.Common.Converters
{
    public class MessageTypeToColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            MessageType msgType = (MessageType)value;

            //FF999999
            Color color = Color.FromArgb(0xff, 0x99, 0x99, 0x99);
            switch (msgType)
            {
                case MessageType.Info:
                    break;
                case MessageType.Warning:
                    color = Colors.Orange;
                    break;
                case MessageType.Success:
                    color = Colors.Green;
                    break;
                case MessageType.Error:
                    color = Colors.Red;
                    break;
                default:
                    break;
            }

            SolidColorBrush brush = new SolidColorBrush(color);
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
