namespace Entities.Exceptions;

public sealed class CompanyCollectionBadRequest : BadRequestException
{
    public CompanyCollectionBadRequest()
        : base("Company collection sent from a client is null.")
    {
    }

    public CompanyCollectionBadRequest(string message) : base(message)
    {
    }

    public CompanyCollectionBadRequest(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}