using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;

namespace UsersService.Application.Users.Commands.DeleteUserCommand
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, int>
    {
        private readonly ILogger<DeleteUserCommand> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteUserCommandHandler(
            ILogger<DeleteUserCommand> logger,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start handling {command}", request.GetType().Name);

            var userEntity = await _unitOfWork.UsersRepository.GetAsync(request.Id, cancellationToken)
                ?? throw new NotImplementedException($"User with id {request.Id} not found");

            _unitOfWork.UsersRepository.Remove(userEntity);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Successfully handled {command}", request.GetType().Name);

            return request.Id;
        }
    }
}
