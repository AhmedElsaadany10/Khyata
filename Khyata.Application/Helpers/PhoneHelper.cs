using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Application.Helpers
{
    public static class PhoneHelper
    {
        public static string NormalizeEgyptianPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return phone;

            phone = phone.Trim().Replace(" ", "");

            if (phone.StartsWith("+20"))
                phone = phone.Substring(3);

            else if (phone.StartsWith("20"))
                phone = phone.Substring(2);

            else if (phone.StartsWith("02"))
                phone = phone.Substring(2);

            if (!phone.StartsWith("0"))
                phone = "0" + phone;

            return phone;
        }
    }
}
