using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects
{
    public class Password : ValueObject
    {
        private string _value;
        private static readonly Regex _passwordRegex = new Regex(
     @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
     RegexOptions.Compiled);

        private Password() { }

        private Password(string value)
        {
            _value = value;
        }

        public static Password Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Password can't be empty");

            if (value.Length < 3)
               throw new ArgumentException("Password must be between 3 and 50 character");

            if (!_passwordRegex.IsMatch(value))
               throw new ArgumentException("Password must contains atleast a minus, a capital, a digit and a minimum of 8 characters");

            return FromPlainText(value);
        }

        public static Password FromPlainText(string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(plainPassword))
                throw new ArgumentException("Password can't be empty");

            var hashedPassword = HashString(plainPassword);
            return new Password(hashedPassword);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return _value;
        }

        public override string ToString() => _value;

        private static string HashString(string str)
        {
            byte[] strByte = Encoding.UTF8.GetBytes(str);
            byte[] hashValue = SHA512.HashData(strByte);

            return Convert.ToHexString(hashValue);
        }
    }
}
