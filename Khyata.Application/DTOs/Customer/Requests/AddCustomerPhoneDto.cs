using System.ComponentModel.DataAnnotations;

namespace Khyata.Application.DTOs.Customer.Requests
{
    public class AddCustomerPhoneDto
    {
        [Required, MaxLength(11)]
        public string Number { get; set; } = default!;

        public bool IsPrimary { get; set; } = false;
    }
}
