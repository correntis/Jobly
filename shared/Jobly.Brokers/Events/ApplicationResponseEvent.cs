namespace Jobly.Brokers.Events
{
    public class ApplicationResponseEvent
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid ApplicationId { get; set; }
        public string ApplicationStatus { get; set; }
        public Guid VacancyId { get; set; }
        public string VacancyTitle { get; set; }
    }
}
