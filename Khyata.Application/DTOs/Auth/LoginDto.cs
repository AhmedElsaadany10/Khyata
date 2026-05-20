using System.ComponentModel.DataAnnotations;

namespace khyata.Application.DTOs.Auth
{
    public class LoginDto
    {
        [Required]
        public string Phone { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;
    }
}
