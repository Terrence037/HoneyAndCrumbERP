using System;
using System.Collections.Generic;

namespace HoneyAndCrumb.Client
{
    public static class Session
    {
        public static int CompanyId { get; set; }
        public static string CompanyCode { get; set; }
        public static string CompanyName { get; set; }
        public static string Username { get; set; }
        public static string FullName { get; set; }
        public static string Role { get; set; }
        public static string SubscriptionPlan { get; set; }
        public static HashSet<string> Permissions { get; private set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private static readonly HashSet<string> MicroModules = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Sales", "Record payment", "Track order status", "Operational reports",
            "Customers", "Menu", "Ingredients"
        };

        private static readonly HashSet<string> SmallModules = new HashSet<string>(MicroModules, StringComparer.OrdinalIgnoreCase)
        {
            "Supply chain", "Payroll", "Financial statements", "Dashboard", "Business intelligence"
        };

        private static readonly HashSet<string> MediumModules = new HashSet<string>(SmallModules, StringComparer.OrdinalIgnoreCase)
        {
            "Human resource", "Branching"
        };

        private static readonly HashSet<string> SuperAdminModules = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Dashboard", "Manage clients", "Subscriptions", "Business intelligence"
        };

        public static void SetPermissions(IEnumerable<string> perms)
        {
            Permissions = perms == null
                ? new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                : new HashSet<string>(perms, StringComparer.OrdinalIgnoreCase);
        }

        public static bool PlanIncludes(string module)
        {
            if (string.Equals(SubscriptionPlan, "Medium", StringComparison.OrdinalIgnoreCase))
                return MediumModules.Contains(module);
            if (string.Equals(SubscriptionPlan, "Small", StringComparison.OrdinalIgnoreCase))
                return SmallModules.Contains(module);
            return MicroModules.Contains(module);
        }

        public static bool CanAccess(string module)
        {
            if (string.IsNullOrEmpty(Role)) return false;

            if (string.Equals(Role, "SuperAdmin", StringComparison.OrdinalIgnoreCase))
                return SuperAdminModules.Contains(module);

            if (!PlanIncludes(module))
                return false;

            if (string.Equals(Role, "Owner", StringComparison.OrdinalIgnoreCase))
                return true;

            return Permissions.Contains(module);
        }

        public static void Clear()
        {
            CompanyId = 0;
            CompanyCode = null;
            CompanyName = null;
            Username = null;
            FullName = null;
            Role = null;
            SubscriptionPlan = null;
            Permissions.Clear();
        }
    }
}