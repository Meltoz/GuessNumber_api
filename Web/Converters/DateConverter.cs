using AutoMapper;
using System.Globalization;
using Web.Constants;

namespace Web.Converters
{
    public class DateConverter : ITypeConverter<string, DateTime?>
    {
        public DateTime? Convert(string source, DateTime? destination, ResolutionContext context)
        {
            return ParseDate(source);
        }

        public static DateTime? ParseDate(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return null;

            return DateTimeOffset.TryParse(source, CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind, out var date)
                    ? date.UtcDateTime
                    : null;
        }
    }
}
