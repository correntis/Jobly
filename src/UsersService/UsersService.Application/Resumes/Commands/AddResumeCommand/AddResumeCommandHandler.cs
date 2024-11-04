using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Entities.NoSQL;
using UsersService.Domain.Exceptions;

namespace UsersService.Application.Resumes.Commands.AddResumeCommand
{
    public class AddResumeCommandHandler : IRequestHandler<AddResumeCommand, string>
    {
        private readonly ILogger<AddResumeCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AddResumeCommandHandler(
            ILogger<AddResumeCommandHandler> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<string> Handle(AddResumeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start handling {command}", request.GetType().Name);

            _ = await _unitOfWork.UsersRepository.GetAsync(request.UserId, cancellationToken)
                ?? throw new EntityNotFoundException($"User with id {request.UserId} not found");

            if (await _unitOfWork.ResumesRepository.GetByAsync(r => r.UserId, request.UserId, cancellationToken) is not null)
            {
                throw new EntityAlreadyExistsException($"Resume for user with id {request.UserId} already exists");
            }

            var resumeEntity = _mapper.Map<ResumeEntity>(request);

            resumeEntity.CreatedAt = DateTime.Now;
            resumeEntity.UpdatedAt = DateTime.Now;

            await _unitOfWork.ResumesRepository.AddAsync(resumeEntity, cancellationToken);

            _logger.LogInformation("Successfully handled {command}", request.GetType().Name);

            return resumeEntity.Id;
        }
    }
}
