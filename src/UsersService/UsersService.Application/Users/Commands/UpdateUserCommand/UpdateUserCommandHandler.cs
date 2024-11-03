using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Exceptions;

namespace UsersService.Application.Users.Commands.UpdateUserCommand
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
    {
        private readonly ILogger<UpdateUserCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserCommandHandler(
            ILogger<UpdateUserCommandHandler> logger,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start handling {command}", nameof(UpdateUserCommand));

            var userEntity = await _unitOfWork.UsersRepository.GetAsync(request.Id, cancellationToken)
                ?? throw new EntityNotFoundException($"User with id {request.Id} not found");

            userEntity.FirstName = request.FirstName;
            userEntity.LastName = request.LastName;
            userEntity.Phone = request.Phone;

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Successfully handled {command}", nameof(UpdateUserCommand));
        }
    }
}
