using Khyata.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Application.Helpers
{
    public static class AdminRoles
    {
        public const string Moderator = "Moderator";
        public const string Admin = "Admin";
        public const string SuperAdmin = "SuperAdmin";

        public static readonly string[] All =
            [ SuperAdmin, Admin, Moderator];
    }
}
