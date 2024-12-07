using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Contexts;
using VacanciesService.Domain.Entities.SQL;

namespace VacanciesService.Application.Interactions.Commands.AddInteractionCommand
{
    public class AddInteractionCommandHandler : IRequestHandler<AddInteractionCommand, Guid>
    {
        private readonly ILogger<AddInteractionCommand> _logger;
        private readonly IVacanciesWriteContext _vacanciesContext;
        private readonly IMapper _mapper;

        public AddInteractionCommandHandler(
            ILogger<AddInteractionCommand> logger,
            IVacanciesWriteContext vacanciesContext,
            IMapper mapper)
        {
            _logger = logger;
            _vacanciesContext = vacanciesContext;
            _mapper = mapper;
        }

        public async Task<Guid> Handle(AddInteractionCommand request, CancellationToken token = default)
        {
            _logger.LogInformation(
               "Start handling {CommandName} for vacancy with ID {VacancyId} and user with ID {UserId}",
               request.GetType().Name,
               request.VacancyId,
               request.UserId);

            var interactionEntity = _mapper.Map<VacancyInteractionEntity>(request);

            interactionEntity.CreatedAt = DateTime.Now;

            await _vacanciesContext.Interactions.AddAsync(interactionEntity, token);

            await _vacanciesContext.SaveChangesAsync(token);

            _logger.LogInformation(
                "Successfully handled {CommandName} for vacancy with ID {VacancyId} and user with ID {UserId}",
                request.GetType().Name,
                request.VacancyId,
                request.UserId);

            return interactionEntity.Id;
        }
    }
}
