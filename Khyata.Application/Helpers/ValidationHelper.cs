using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Khyata.Application.Helpers
{
    public static class ValidationHelper
    {
        public static bool IsEgyptianPhone(string phone)
        {
            return Regex.IsMatch(phone, @"^01[0125][0-9]{8}$");
        }

        public static bool IsStrongPassword(string password)
        {
            return Regex.IsMatch(password,
                @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{8,}$");
        }
    }
}
