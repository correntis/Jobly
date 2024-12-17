using MediatR;

namespace MessagesService.Application.Messages.Commands.ReadMessage
{
    public sealed record ReadMessageCommand(string MessageId) : IRequest;
}
