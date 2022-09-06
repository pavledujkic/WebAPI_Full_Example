namespace Entities.Exceptions;

public sealed class IdParametersBadRequestException : BadRequestException
{
    public IdParametersBadRequestException() : base("Parameter ids is null")
    {
    }

    public IdParametersBadRequestException(string message) : base(message)
    {
    }

    public IdParametersBadRequestException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}