namespace Entities.Exceptions;

public abstract class BadRequestException : Exception
{
    protected BadRequestException(string message)
        : base(message)
    {
    }

    protected BadRequestException() : base()
    {
    }

    protected BadRequestException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}