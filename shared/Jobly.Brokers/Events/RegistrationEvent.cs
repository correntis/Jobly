namespace Jobly.Brokers.Models
{
    public class RegistrationEvent
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
    }
}
