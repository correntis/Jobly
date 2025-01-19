using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Entities.NoSQL;
using UsersService.Domain.Exceptions;
using UsersService.Domain.Models;

namespace UsersService.Application.Resumes.Commands.AddResume
{
    public class AddResumeCommandHandler : IRequestHandler<AddResumeCommand, Resume>
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

        public async Task<Resume> Handle(AddResumeCommand request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Start handling {CommandName} for user with ID {UserId}", request.GetType().Name, request.UserId);

            _ = await _unitOfWork.UsersRepository.GetByIdAsync(request.UserId)
                ?? throw new EntityNotFoundException($"User with id {request.UserId} not found");

            var existingResumeEntity = await _unitOfWork.ResumesRepository.GetByAsync(r => r.UserId, request.UserId, cancellationToken);

            if(existingResumeEntity is not null)
            {
                throw new EntityAlreadyExistsException($"Resume for user with id {request.UserId} already exists");
            }

            var resumeEntity = _mapper.Map<ResumeEntity>(request);

            resumeEntity.CreatedAt = DateTime.UtcNow;
            resumeEntity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.ResumesRepository.AddAsync(resumeEntity, cancellationToken);

            _logger.LogInformation("Successfully handled {CommandName} for user with ID {UserId}", request.GetType().Name, request.UserId);

            return _mapper.Map<Resume>(resumeEntity);
        }
    }
}
