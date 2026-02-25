namespace Domain.ValueObjects
{
    public class Code : ValueObject
    {
        public string Value { get; private set; }

        private Code(string value)
        {
            Value = value;
        }
        public static Code Create(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Pseudo can't be empty");

            return new Code(code.Trim());
        }

        public static Code Generate(int codeLength = 10)
        {
            Random rang = new Random();
            string codeGenerated = "";
            int randValue;
            char letter;
            for (int i = 0; i < codeLength; i++)
            {
                randValue = rang.Next(0, 26);

                letter = Convert.ToChar(randValue + 65);
                codeGenerated += letter;
            }
            return new Code(codeGenerated);
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

        public static implicit operator string(Code code) => code.Value;
    }
}
