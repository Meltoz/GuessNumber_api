using System.Text.RegularExpressions;

namespace Domain.ValueObjects
{
    public class Mail : ValueObject
    {
        private static readonly Regex EmailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$", 
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private string _value;

        private Mail() { }

        private Mail(string value)
        {
            _value = value;
        }

        public static Mail Create(string mail)
        {
            if (string.IsNullOrWhiteSpace(mail))
                throw new ArgumentException("Mail can't be empty");

            mail = mail.Trim();

            if (!EmailRegex.IsMatch(mail))
                throw new ArgumentException("L'adresse email n'est pas valide.");

            return new Mail(mail);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return _value.ToLowerInvariant();
        }

        public override string ToString() => _value;

        public bool Contains(string substring)
        {
            return _value != null && _value.Contains(substring, StringComparison.OrdinalIgnoreCase);
        }

        public static implicit operator string(Mail mail) => mail._value;

    }
}
