using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Exceptions;

namespace UsersService.Application.Resumes.Commands.DeleteResumeCommand
{
    public class DeleteResumeCommandHandler : IRequestHandler<DeleteResumeCommand, string>
    {
        private readonly ILogger<DeleteResumeCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeleteResumeCommandHandler(
            ILogger<DeleteResumeCommandHandler> logger,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(DeleteResumeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start handling {command}", request.GetType().Name);

            _ = _unitOfWork.ResumesRepository.GetAsync(request.Id, cancellationToken)
                ?? throw new EntityNotFoundException($"Resume with id {request.Id} not found");

            await _unitOfWork.ResumesRepository.DeleteAsync(request.Id, cancellationToken);

            _logger.LogInformation("Successfully handled {command}", request.GetType().Name);

            return request.Id;
        }
    }
}
