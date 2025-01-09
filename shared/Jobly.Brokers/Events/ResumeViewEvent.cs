namespace Jobly.Brokers.Events
{
    public class ResumeViewEvent
    {
        public Guid UserId { get; set; }
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
    }
}
