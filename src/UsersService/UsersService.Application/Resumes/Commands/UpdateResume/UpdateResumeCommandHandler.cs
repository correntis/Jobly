using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Exceptions;
using UsersService.Domain.Models;

namespace UsersService.Application.Resumes.Commands.UpdateResume
{
    public class UpdateResumeCommandHandler : IRequestHandler<UpdateResumeCommand, Resume>
    {
        private readonly ILogger<UpdateResumeCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateResumeCommandHandler(
            ILogger<UpdateResumeCommandHandler> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Resume> Handle(UpdateResumeCommand request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Start handling {CommandName} for resume with ID {ResumeId}", request.GetType().Name, request.Id);

            var resumeEntity = await _unitOfWork.ResumesRepository.GetAsync(request.Id, cancellationToken)
                ?? throw new EntityNotFoundException($"Resume with id {request.Id} not found");

            _mapper.Map(request, resumeEntity);

            resumeEntity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.ResumesRepository.UpdateAsync(resumeEntity, cancellationToken);

            _logger.LogInformation("Successfully handled {CommandName} for resume with ID {ResumeId}", request.GetType().Name, request.Id);

            return _mapper.Map<Resume>(resumeEntity);
        }
    }
}
