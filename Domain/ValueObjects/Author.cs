using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public class Author : ValueObject
    {
        public string Value { get; private set; }

        private Author() { }

        private Author(string value) 
        {
            Value = value;
        }

        public static Author Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Pseudo can't be empty");

            if (value.Length < 3 || value.Length > 100)
                throw new ArgumentException("Must be between 3 and 100 character");

            return new Author(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value.ToLowerInvariant();
        }

        public override string ToString() => Value;

        public bool Contains(string substring)
        {
            return Value != null && Value.Contains(substring, StringComparison.OrdinalIgnoreCase);
        }

        public static implicit operator string(Author author) => author.Value;
    }
}
