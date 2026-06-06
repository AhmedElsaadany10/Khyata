using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Application.DTOs.Order.Financial
{
    public class FinancialReportResponseDto
    {
        public decimal TotalOrdersAmount { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public decimal TotalRemainingAmount { get; set; }
        public decimal TotalCancelledAmount { get; set; }
        public int NumberOfOrders { get; set; }
        public int NumberOfCancelledOrders { get; set; }
        public decimal ActiveRevenue { get; set; } // Non-cancelled orders sum
    }
}
