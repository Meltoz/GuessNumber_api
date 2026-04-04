namespace Domain.Exceptions;

public class ArgumentEmptyException : Exception
{
    public ArgumentEmptyException(string propertyName) : 
        base($"Property {propertyName} is empty.")
    { }

    public static void ThrowIfEmpty<T>(IReadOnlyCollection<T> collection, string propertyName) 
    {
        if (collection.Count == 0)
            throw new ArgumentEmptyException(propertyName);
    }
}