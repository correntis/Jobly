using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Exceptions;

namespace UsersService.Application.Resumes.Commands.DeleteResume
{
    public class DeleteResumeCommandHandler : IRequestHandler<DeleteResumeCommand, string>
    {
        private readonly ILogger<DeleteResumeCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteResumeCommandHandler(
            ILogger<DeleteResumeCommandHandler> logger,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(DeleteResumeCommand request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Start handling {CommandName} for resume with ID {ResumeId}", request.GetType().Name, request.Id);

            _ = await _unitOfWork.ResumesRepository.GetAsync(request.Id, cancellationToken)
                ?? throw new EntityNotFoundException($"Resume with id {request.Id} not found");

            await _unitOfWork.ResumesRepository.DeleteAsync(request.Id, cancellationToken);

            _logger.LogInformation("Successfully handled {CommandName} for resume with ID {ResumeId}", request.GetType().Name, request.Id);

            return request.Id;
        }
    }
}
