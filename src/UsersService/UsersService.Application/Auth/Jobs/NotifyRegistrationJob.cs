using Jobly.Brokers.Abstractions;
using Jobly.Brokers.Models;
using Microsoft.Extensions.Logging;

namespace UsersService.Application.Auth.Jobs
{
    public class NotifyRegistrationJob
    {
        private readonly ILogger<NotifyRegistrationJob> _logger;
        private readonly IBrokerProcuder _brokerProcuder;

        public NotifyRegistrationJob(
            ILogger<NotifyRegistrationJob> logger,
            IBrokerProcuder brokerProcuder)
        {
            _logger = logger;
            _brokerProcuder = brokerProcuder;
        }

        public async Task ExecuteAsync(RegistrationEvent registrationEvent)
        {
            _logger.LogInformation("[Job] Start execute job {JobName}", nameof(NotifyRegistrationJob));

            await _brokerProcuder.SendAsync(registrationEvent);
        }
    }
}
