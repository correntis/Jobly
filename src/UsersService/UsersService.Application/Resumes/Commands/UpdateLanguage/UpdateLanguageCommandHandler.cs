using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Entities.NoSQL;
using UsersService.Domain.Exceptions;

namespace UsersService.Application.Resumes.Commands.UpdateLanguage
{
    public class UpdateLanguageCommandHandler : IRequestHandler<UpdateLanguageCommand, string>
    {
        private readonly ILogger<UpdateLanguageCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateLanguageCommandHandler(
            ILogger<UpdateLanguageCommandHandler> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<string> Handle(UpdateLanguageCommand request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Start handling {CommandName} for resume with ID {ResumeId}", request.GetType().Name, request.Id);

            _ = await _unitOfWork.ResumesRepository.GetAsync(request.Id, cancellationToken)
                ?? throw new EntityNotFoundException($"Resume with id {request.Id} not found");

            var languagesEntities = _mapper.Map<List<LanguageEntity>>(request.Languages);

            if(languagesEntities.Count == 0)
            {
                languagesEntities = null;
            }

            await _unitOfWork.ResumesRepository.UpdateByAsync(
                request.Id,
                r => r.Languages,
                languagesEntities,
                cancellationToken);

            _logger.LogInformation("Successfully handled {CommandName} for resume with ID {ResumeId}", request.GetType().Name, request.Id);

            return request.Id;
        }
    }
}
