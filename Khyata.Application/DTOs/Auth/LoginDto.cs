using System.ComponentModel.DataAnnotations;

namespace Khyata.Application.DTOs.Auth
{
    public class LoginDto
    {
        [Required]
        public string Phone { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;
    }
}
