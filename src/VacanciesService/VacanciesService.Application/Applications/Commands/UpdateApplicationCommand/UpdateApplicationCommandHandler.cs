using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Contexts;
using VacanciesService.Domain.Exceptions;

namespace VacanciesService.Application.Applications.Commands.UpdateApplicationCommand
{
    public class UpdateApplicationCommandHandler : IRequestHandler<UpdateApplicationCommand, int>
    {
        private readonly ILogger<UpdateApplicationCommandHandler> _logger;
        private readonly IVacanciesWriteContext _vacanciesContext;

        public UpdateApplicationCommandHandler(
            ILogger<UpdateApplicationCommandHandler> logger,
            IVacanciesWriteContext vacanciesContext)
        {
            _logger = logger;
            _vacanciesContext = vacanciesContext;
        }

        public async Task<int> Handle(UpdateApplicationCommand request, CancellationToken token)
        {
            _logger.LogInformation(
                "Start handling {CommandName} for application with ID {ApplicationId}",
                request.GetType().Name,
                request.Id);

            var applicationEntity = await _vacanciesContext.Applications.FirstOrDefaultAsync(v => v.Id == request.Id);

            if (applicationEntity is null)
            {
                throw new EntityNotFoundException($"Application with ID {request.Id} not found");
            }

            applicationEntity.Status = request.Status;
            applicationEntity.AppliedAt = DateTime.Now;

            await _vacanciesContext.SaveChangesAsync(token);

            _logger.LogInformation(
                "Successfully handled {CommandName} for application with ID {ApplicationId}",
                request.GetType().Name,
                request.Id);

            return applicationEntity.Id;
        }
    }
}
