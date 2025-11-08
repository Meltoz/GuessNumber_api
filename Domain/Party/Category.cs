namespace Domain.Party
{
    public class Category
    {
        public Guid Id { get; private set; } = Guid.Empty;

        public string Name { get; private set; } = string.Empty;

        private Category() { }

        public Category(string name)
        {
            ChangeName(name);
        }

        public Category(Guid id, string name) : this(name)
        {
            if (Id == Guid.Empty && id != Guid.Empty)
                Id = id;
        }

        public void ChangeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name must be set");
            
            if (name.Length > 500)
                throw new ArgumentOutOfRangeException("name");

            Name = name;
        }

    }
}
