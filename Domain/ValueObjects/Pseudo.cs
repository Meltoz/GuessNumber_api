namespace Domain.ValueObjects
{
    public class Pseudo : ValueObject
    { 
        public string Value { get; private set; }

        private Pseudo(string value)
        {
            Value = value;
        }

        public static Pseudo Create(string pseudo)
        {
            if (string.IsNullOrWhiteSpace(pseudo))
                throw new ArgumentException("Pseudo can't be empty");

            return new Pseudo(pseudo.Trim());
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

        public static implicit operator string(Pseudo pseudo) => pseudo.Value;
    }
}
