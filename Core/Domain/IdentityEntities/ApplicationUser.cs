using Microsoft.AspNetCore.Identity;

namespace Core.Domain.IdentityEntities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? PersonName { get; set; }
}