using MediatR;

namespace MessagesService.Application.Messages.Commands.EditMessage
{
    public sealed record EditMessageCommand(string MessageId, string Content) : IRequest;
}
