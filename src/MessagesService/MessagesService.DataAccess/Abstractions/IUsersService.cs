namespace MessagesService.DataAccess.Abstractions
{
    public interface IUsersService
    {
        Task<long?> GetUserTelegramChatIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<bool> UpdateTelegramChatIdAsync(Guid userId, long telegramChatId, CancellationToken cancellationToken = default);
    }
}

