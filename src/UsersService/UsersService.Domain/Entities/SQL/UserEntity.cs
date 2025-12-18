using Microsoft.AspNetCore.Identity;

namespace UsersService.Domain.Entities.SQL
{
    public class UserEntity : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsFullRegistration { get; set; } = true;
        public long? TelegramChatId { get; set; }
    }
}
