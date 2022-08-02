using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class EmployeeForUpdateDto
    {
        public string Name { get; set; } = default!;
        public int Age { get; set; }
        public string Position { get; set; } = default!;
    }
}
