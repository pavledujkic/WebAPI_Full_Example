using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects
{
    public abstract class CompanyForManipulationDto
    {
        [Required(ErrorMessage = "Company name is a required field.")]
        [MaxLength(50, ErrorMessage = "Maximum length for the Name is 50 characters.")]
        public string Name { get; set; } = default!;

        [Required(ErrorMessage = "Company Address is a required field.")]
        [MaxLength(150, ErrorMessage = "Maximum length for the Name is 150 characters.")]
        public string Address { get; set; } = default!;

        [Required(ErrorMessage = "Company Country is a required field.")]
        [MaxLength(50, ErrorMessage = "Maximum length for the Name is 50 characters.")]
        public string Country { get; set; } = default!;

        public IEnumerable<EmployeeForCreationDto>? Employees { get; set; }
    }
}
