using Microsoft.AspNetCore.Identity;

namespace UsersService.Domain.Entities.SQL
{
    public class RoleEntity : IdentityRole<Guid>
    {
        public string Description { get; set; }
    }
}
