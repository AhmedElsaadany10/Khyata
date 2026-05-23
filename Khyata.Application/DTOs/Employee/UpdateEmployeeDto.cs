using System.ComponentModel.DataAnnotations;

namespace Khyata.Application.DTOs.Employee
{
    public class UpdateEmployeeDto
    {
        [MaxLength(200)]
        public string? Name { get; set; }

        [MinLength(8)]
        public string? NewPassword { get; set; }

        public string? CurrentPassword { get; set; }
    }
}
