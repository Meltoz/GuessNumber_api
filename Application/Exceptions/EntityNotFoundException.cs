namespace Application.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(Guid id) : base($"Entity with id='{id}' not found")
        {
        }

        public EntityNotFoundException() : base("An Entity was not found")
        {

        }
    }
}
