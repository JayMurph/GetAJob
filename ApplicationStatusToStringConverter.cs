using System.Globalization;

namespace ApplicationOrganizer
{
    /// <summary>
    /// Converts an ApplicationStatus enum value to a string representation and vice versa
    /// </summary>
    public class ApplicationStatusToStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts an ApplicationStatus value to a string representation 
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="targetType">Assumed to by ApplicationStatus</param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>String representation of ApplicationStatus value</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ApplicationStatus status = (ApplicationStatus)value;
            return nameof(status);
        }

        /// <summary>
        /// Converts a string to an ApplicationStatus value
        /// </summary>
        /// <param name="value">String to convert</param>
        /// <param name="targetType">Assumed to be string</param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.Parse<ApplicationStatus>(value as string);
        }
    }
}
