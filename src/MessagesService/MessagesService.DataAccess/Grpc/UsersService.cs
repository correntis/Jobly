using Jobly.Protobufs.Users;
using Jobly.Protobufs.Users.Client;
using MessagesService.DataAccess.Abstractions;
using Microsoft.Extensions.Logging;

namespace MessagesService.DataAccess.Grpc
{
    public class UsersService : IUsersService
    {
        private readonly UsersGrpcService.UsersGrpcServiceClient _client;
        private readonly ILogger<UsersService> _logger;

        public UsersService(
            ILogger<UsersService> logger,
            UsersGrpcService.UsersGrpcServiceClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task<long?> GetUserTelegramChatIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var request = new GetUserTelegramChatIdRequest()
            {
                UserId = userId.ToString()
            };

            _logger.LogInformation(
                "[GRPC] Start handling {RequestName} for user {UserId}",
                typeof(GetUserTelegramChatIdRequest).Name,
                userId);

            try
            {
                var response = await _client.GetUserTelegramChatIdAsync(request, cancellationToken: cancellationToken);

                _logger.LogInformation(
                    "[GRPC] Successfully handled {RequestName} for user {UserId}, hasTelegramChatId: {HasTelegramChatId}",
                    typeof(GetUserTelegramChatIdRequest).Name,
                    userId,
                    response.HasTelegramChatId);

                return response.HasTelegramChatId ? response.TelegramChatId : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GRPC] Error getting TelegramChatId for user {UserId}", userId);
                return null;
            }
        }

        public async Task<bool> UpdateTelegramChatIdAsync(Guid userId, long telegramChatId, CancellationToken cancellationToken = default)
        {
            var request = new UpdateTelegramChatIdRequest
            {
                UserId = userId.ToString(),
                TelegramChatId = telegramChatId
            };

            _logger.LogInformation(
                "[GRPC] Start handling {RequestName} for user {UserId}",
                typeof(UpdateTelegramChatIdRequest).Name,
                userId);

            try
            {
                var response = await _client.UpdateTelegramChatIdAsync(request, cancellationToken: cancellationToken);

                _logger.LogInformation(
                    "[GRPC] Successfully handled {RequestName} for user {UserId}, success: {Success}",
                    typeof(UpdateTelegramChatIdRequest).Name,
                    userId,
                    response.Success);

                return response.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GRPC] Error updating TelegramChatId for user {UserId}", userId);
                return false;
            }
        }
    }
}

