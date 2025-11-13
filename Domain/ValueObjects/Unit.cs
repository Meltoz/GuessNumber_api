namespace Domain.ValueObjects
{
    public class Unit : ValueObject
    {
        public string Value { get; private set; }

        private Unit() 
        {
            Value = string.Empty;
        }

        private Unit(string value)
        {
            Value = value; 
        }

        public static Unit Create(string value) {
            return new Unit(value);
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

        public static implicit operator string(Unit unit) => unit.Value;
    }
}
