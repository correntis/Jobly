namespace Jobly.Brokers.Events
{
    public class RecomendVacancyEvent
    {
        public Guid UserId { get; set; }
        public Guid VacancyId { get; set; }
        public string VacancyName { get; set; }
        public float MatchScore { get; set; }
    }
}
