using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Domain.Enums
{
    public static class AdminPolicies
    {
        public const string AnyAdmin = "AnyAdmin";
        public const string SuperAdminOnly = "SuperAdminOnly";
        public const string ModeratorOrAbove = "ModeratorOrAbove";
    }
}
