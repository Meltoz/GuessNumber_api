using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    public class Libelle : ValueObject
    {
        public string Value { get; private set; }

        private Libelle() {
            Value = string.Empty;
        }

        private Libelle(string value)
        {
            Value = value; 
        }

        public static Libelle Create(string value)
        {
            return new Libelle(value);
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

        public static implicit operator string(Libelle libelle) => libelle.Value;
    }
}
