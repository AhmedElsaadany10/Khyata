using System.ComponentModel.DataAnnotations;

namespace Khyata.Application.DTOs.Auth
{
    public class RegisterOwnerDto
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = default!;

        [Required, MaxLength(20)]
        public string Phone { get; set; } = default!;

        [Required, MinLength(8)]
        public string Password { get; set; } = default!;

        [Required, MaxLength(200)]
        public string WorkspaceName { get; set; } = default!;
    }
}
