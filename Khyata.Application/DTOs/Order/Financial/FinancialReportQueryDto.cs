using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Application.DTOs.Order.Financial
{
    public class FinancialReportQueryDto
    {
        public string Period { get; set; } = "monthly"; // daily | weekly | monthly | yearly | custom
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
