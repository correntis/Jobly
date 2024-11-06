namespace UsersService.Domain.Entities.NoSQL
{
    public class TokenEntity
    {
        public string RefreshToken { get; set; }
        public int UserId { get; set; }
        public IEnumerable<string> UserRoles { get; set; }
    }
}
