namespace Entities.Exceptions;

public sealed class EmployeeNotFoundException : NotFoundException
{
    public EmployeeNotFoundException(Guid employeeId)
        : base($"Employee with id: {employeeId} doesn't exist in the database.")
    {
    }

    public EmployeeNotFoundException(string message) : base(message)
    {
    }

    public EmployeeNotFoundException() : base()
    {
    }

    public EmployeeNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}