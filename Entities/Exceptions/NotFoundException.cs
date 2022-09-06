namespace Entities.Exceptions;

public abstract class NotFoundException : Exception
{
    protected NotFoundException(string message)
        : base(message)
    {
    }

    protected NotFoundException() : base()
    {
    }

    protected NotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}