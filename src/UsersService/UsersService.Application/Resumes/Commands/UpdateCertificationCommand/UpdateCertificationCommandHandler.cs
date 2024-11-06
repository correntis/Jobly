using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Application.Resumes.Commands.DeleteResumeCommand;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Entities.NoSQL;
using UsersService.Domain.Exceptions;

namespace UsersService.Application.Resumes.Commands.UpdateCertificationCommand
{
    public class UpdateCertificationCommandHandler : IRequestHandler<UpdateCertificationCommand, string>
    {
        private readonly ILogger<DeleteResumeCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateCertificationCommandHandler(
            ILogger<DeleteResumeCommandHandler> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<string> Handle(UpdateCertificationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start handling {CommandName} for resume with ID {ResumeId}", request.GetType().Name, request.Id);

            _ = _unitOfWork.ResumesRepository.GetAsync(request.Id, cancellationToken)
                ?? throw new EntityNotFoundException($"Resume with id {request.Id} not found");

            var certificationEntities = _mapper.Map<List<CertificationEntity>>(request.Certifications);

            if (certificationEntities.Count == 0)
            {
                certificationEntities = null;
            }

            await _unitOfWork.ResumesRepository.UpdateByAsync(
                request.Id,
                r => r.Certifications,
                certificationEntities,
                cancellationToken);

            _logger.LogInformation("Successfully handled {CommandName} for resume with ID {ResumeId}", request.GetType().Name, request.Id);

            return request.Id;
        }
    }
}
