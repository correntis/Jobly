using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Exceptions;

namespace UsersService.Application.Users.Commands.UpdateTelegramChatId
{
    public class UpdateTelegramChatIdCommandHandler : IRequestHandler<UpdateTelegramChatIdCommand, Guid>
    {
        private readonly ILogger<UpdateTelegramChatIdCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTelegramChatIdCommandHandler(
            ILogger<UpdateTelegramChatIdCommandHandler> logger,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(UpdateTelegramChatIdCommand request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Start handling {CommandName} for user with ID {UserId}", request.GetType().Name, request.UserId);

            var userEntity = await _unitOfWork.UsersRepository.GetByIdAsync(request.UserId)
                ?? throw new EntityNotFoundException($"User with id {request.UserId} not found");

            userEntity.TelegramChatId = request.TelegramChatId;

            var updateResult = await _unitOfWork.UsersRepository.UpdateAsync(userEntity);
            
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                _logger.LogError("Failed to update TelegramChatId for user {UserId}: {Errors}", request.UserId, errors);
                throw new InvalidOperationException($"Failed to update TelegramChatId: {errors}");
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully handled {CommandName} for user with ID {UserId}, TelegramChatId: {TelegramChatId}", 
                request.GetType().Name, 
                userEntity.Id, 
                userEntity.TelegramChatId);

            return request.UserId;
        }
    }
}

