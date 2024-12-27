namespace Jobly.Brokers.Events
{
    public class VacancyApplicationEvent
    {
        public Guid UserId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid VacacnyId { get; set; }
        public Guid ApplicationId { get; set; }
    }
}
