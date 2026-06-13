using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Application.DTOs.Customer.Responses
{
    public class CustomerOrderSummaryDto
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalPaid { get; set; }
        public string Status { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}
