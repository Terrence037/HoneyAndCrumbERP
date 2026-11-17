using Microsoft.AspNetCore.Identity;

namespace ERP.infrastructure.data;

public class ApplicationUser : IdentityUser
{
    public int? CompanyId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = "Staff"; // SuperAdmin, Owner, Manager, Staff
    public string Permissions { get; set; } = string.Empty; // comma-separated module names
    public bool IsActive { get; set; } = true;
}