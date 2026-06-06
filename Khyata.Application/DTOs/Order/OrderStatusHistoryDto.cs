using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Application.DTOs.Order
{
    public class OrderStatusHistoryDto

    {
        public string FromStatus { get; set; } = string.Empty;

        public string ToStatus { get; set; } = string.Empty;

        public string UpdatedBy { get; set; } = string.Empty;

        public DateTime UpdatedAt { get; set; }

    }
}
