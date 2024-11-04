using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Entities.NoSQL;
using UsersService.Domain.Exceptions;

namespace UsersService.Application.Resumes.Commands.UpdateEducationCommand
{
    public class UpdateEducationCommandHandler : IRequestHandler<UpdateEducationCommand, string>
    {
        private readonly ILogger<UpdateEducationCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateEducationCommandHandler(
            ILogger<UpdateEducationCommandHandler> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<string> Handle(UpdateEducationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start handling {command}", request.GetType().Name);

            _ = _unitOfWork.ResumesRepository.GetAsync(request.Id, cancellationToken)
                ?? throw new EntityNotFoundException($"Resume with id {request.Id} not found");

            var educationEntities = _mapper.Map<List<EducationEntity>>(request.Educations);

            if (educationEntities.Count == 0)
            {
                educationEntities = null;
            }

            await _unitOfWork.ResumesRepository.UpdateByAsync(
                request.Id,
                r => r.Educations,
                educationEntities,
                cancellationToken);

            _logger.LogInformation("Successfully handled {command}", request.GetType().Name);

            return request.Id;
        }
    }
}
