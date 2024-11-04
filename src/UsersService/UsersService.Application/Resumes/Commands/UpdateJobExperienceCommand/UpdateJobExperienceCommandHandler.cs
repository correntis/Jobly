using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Entities.NoSQL;
using UsersService.Domain.Exceptions;

namespace UsersService.Application.Resumes.Commands.UpdateJobExperienceCommand
{
    public class UpdateJobExperienceCommandHandler : IRequestHandler<UpdateJobExperienceCommand, string>
    {
        private readonly ILogger<UpdateJobExperienceCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateJobExperienceCommandHandler(
            ILogger<UpdateJobExperienceCommandHandler> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<string> Handle(UpdateJobExperienceCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start handling {command}", request.GetType().Name);

            _ = _unitOfWork.ResumesRepository.GetAsync(request.Id, cancellationToken)
                ?? throw new EntityNotFoundException($"Resume with id {request.Id} not found");

            var jobExpereincesEntities = _mapper.Map<List<JobExpirienceEntity>>(request.JobExpiriences);

            if (jobExpereincesEntities.Count == 0)
            {
                jobExpereincesEntities = null;
            }

            await _unitOfWork.ResumesRepository.UpdateByAsync(
                request.Id,
                r => r.JobExpiriences,
                jobExpereincesEntities,
                cancellationToken);

            _logger.LogInformation("Successfully handled {command}", request.GetType().Name);

            return request.Id;
        }
    }
}
