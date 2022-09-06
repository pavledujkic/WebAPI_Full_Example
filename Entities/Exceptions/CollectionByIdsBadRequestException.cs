namespace Entities.Exceptions;

public sealed class CollectionByIdsBadRequestException : BadRequestException
{
    public CollectionByIdsBadRequestException() : base("Collection count mismatch comparing to ids.")
    {
    }

    public CollectionByIdsBadRequestException(string message) : base(message)
    {
    }

    public CollectionByIdsBadRequestException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}