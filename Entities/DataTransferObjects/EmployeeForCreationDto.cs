namespace Entities.DataTransferObjects
{
    public class EmployeeForCreationDto
    {
        public string Name { get; set; } = default!;
        public int Age { get; set; } = default!;
        public string Position { get; set; } = default!;
    }
}
