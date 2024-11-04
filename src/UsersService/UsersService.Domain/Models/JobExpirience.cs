namespace UsersService.Domain.Models
{
    public class JobExpirience
    {
        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> Responsibilities { get; set; }
    }
}
