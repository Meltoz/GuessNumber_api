namespace Domain.ValueObjects
{
    public class Token : ValueObject
    {
        public string Value { get; private set; }

        private Token() { }

        private Token(string value)
        {
            Value = value;
        }

        public static Token Create(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Pseudo can't be empty");

            return new Token(token.Trim());
        }

        public override string ToString() => Value;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value.ToLowerInvariant();
        }

        public bool Contains(string substring)
        {
            return Value != null && Value.Contains(substring, StringComparison.OrdinalIgnoreCase);
        }

        public static implicit operator string(Token token) => token.Value;
    }
}
