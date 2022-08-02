namespace Entities.DataTransferObjects
{
    public class CompanyForCreationDto
    {
        public string Name { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string Country { get; set; } = default!;
        public IEnumerable<EmployeeForCreationDto>? Employees { get; set; }
    }
}
