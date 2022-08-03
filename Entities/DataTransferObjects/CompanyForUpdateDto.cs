using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class CompanyForUpdateDto
    {
        public string Name { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string Country { get; set; } = default!;
        public IEnumerable<EmployeeForCreationDto> Employees { get; set; } = default!;
    }
}
