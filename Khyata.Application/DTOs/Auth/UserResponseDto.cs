namespace Khyata.Application.DTOs.Auth
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;

        public string Phone { get; set; } = default!;

        
        public string Role { get; set; } = default!;
        public string WorkspaceName { get; set; } = default!;

    }
}