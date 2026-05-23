namespace Khyata.Application.DTOs.Employee
{
    public class EmployeeResponseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;

        public string Phone { get; set; } = default!;

        public string Role { get; set; } = default!;

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
