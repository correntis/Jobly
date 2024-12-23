using MediatR;
using MessagesService.Core.Models;

namespace MessagesService.Application.Messages.Commands.ReadMessage
{
    public sealed record ReadMessageCommand(string MessageId) : IRequest<Message>;
}
