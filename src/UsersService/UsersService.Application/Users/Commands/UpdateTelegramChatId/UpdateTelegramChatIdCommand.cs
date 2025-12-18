using MediatR;

namespace UsersService.Application.Users.Commands.UpdateTelegramChatId
{
    public sealed record UpdateTelegramChatIdCommand(
        Guid UserId,
        long TelegramChatId) : IRequest<Guid>;
}

