namespace Domain.Exceptions;

public class InvalidScoreUpdateException : Exception
{
    public InvalidScoreUpdateException(string? message) : base(message)
    {
        
    }
}