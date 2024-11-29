using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Exceptions;

namespace UsersService.Application.Users.Commands.DeleteUserCommand
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Guid>
    {
        private readonly ILogger<DeleteUserCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteUserCommandHandler(
            ILogger<DeleteUserCommandHandler> logger,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(DeleteUserCommand request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Start handling {CommandName} for user with ID {UserId}", request.GetType().Name, request.Id);

            var userEntity = await _unitOfWork.UsersRepository.FindByIdAsync(request.Id.ToString())
                ?? throw new EntityNotFoundException($"User with id {request.Id} not found");

            await _unitOfWork.UsersRepository.DeleteAsync(userEntity);

            _logger.LogInformation("Successfully handled {CommandName} for user with ID {UserId}", request.GetType().Name, request.Id);

            return request.Id;
        }
    }
}
