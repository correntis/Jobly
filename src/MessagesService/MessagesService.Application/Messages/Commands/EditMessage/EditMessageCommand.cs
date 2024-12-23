using MediatR;
using MessagesService.Core.Models;

namespace MessagesService.Application.Messages.Commands.EditMessage
{
    public sealed record EditMessageCommand(string MessageId, string Content) : IRequest<Message>;
}
