using Microsoft.AspNetCore.Identity;

namespace Deobely_sync.Model
{
    public class RoleModel : IdentityRole<Guid>
    {
        public string? Description { get; set; }
    }
}
