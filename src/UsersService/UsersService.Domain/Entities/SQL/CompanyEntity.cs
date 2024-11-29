namespace UsersService.Domain.Entities.SQL
{
    public class CompanyEntity : BaseEntity
    {
        public Guid UserId { get; set; }
        public string LogoPath { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string WebSite { get; set; }
        public string Type { get; set; }
        public virtual UserEntity User { get; set; }
    }
}
