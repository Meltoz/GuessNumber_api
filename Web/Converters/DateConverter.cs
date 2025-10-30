using AutoMapper;
using Web.Constants;

namespace Web.Converters
{
    public class DateConverter : ITypeConverter<string, DateTime?>
    {
        public DateTime? Convert(string source, DateTime? destination, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source))
                return null;

            if (DateTime.TryParseExact(source, ApiConstants.DefaultFormatDate, null, System.Globalization.DateTimeStyles.None, out var result))
                return result;

            return null;
        }
    }
}
