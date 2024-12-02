using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Contexts;
using VacanciesService.Domain.Exceptions;

namespace VacanciesService.Application.Applications.Commands.UpdateApplicationCommand
{
    public class UpdateApplicationCommandHandler : IRequestHandler<UpdateApplicationCommand, Guid>
    {
        private readonly ILogger<UpdateApplicationCommandHandler> _logger;
        private readonly IVacanciesWriteContext _vacanciesContext;
        private readonly IMapper _mapper;

        public UpdateApplicationCommandHandler(
            ILogger<UpdateApplicationCommandHandler> logger,
            IVacanciesWriteContext vacanciesContext,
            IMapper mapper)
        {
            _logger = logger;
            _vacanciesContext = vacanciesContext;
            _mapper = mapper;
        }

        public async Task<Guid> Handle(UpdateApplicationCommand request, CancellationToken token)
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

            _mapper.Map(request, applicationEntity);

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
