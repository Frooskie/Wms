using Microsoft.AspNetCore.Identity;

namespace Wms.Core.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}