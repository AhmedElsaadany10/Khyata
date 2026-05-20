using System.ComponentModel.DataAnnotations;

namespace khyata.Application.DTOs.Employee
{
    public class CreateEmployeeDto
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = default!;

        [Required, MaxLength(20)]
        public string Phone { get; set; } = default!;

        [Required, MinLength(8)]
        public string Password { get; set; } = default!;
    }
}
