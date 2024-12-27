using MediatR;
using MessagesService.Core.Models;

namespace MessagesService.Application.Messages.Commands.SendMessage
{
    public sealed record SaveMessageCommand(
        string ChatId,
        Guid SenderId,
        Guid RecipientId,
        string Content) : IRequest<Message>;
}
