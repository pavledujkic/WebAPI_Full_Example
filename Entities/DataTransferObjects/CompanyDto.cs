namespace Entities.DataTransferObjects;

public class CompanyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string FullAddress { get; set; } = default!;
}