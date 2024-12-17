using MediatR;

namespace MessagesService.Application.Messages.Commands.SendMessage
{
    public sealed record SendMessageCommand(
        Guid SenderId,
        Guid RecipientId,
        Guid ApplicationId,
        Guid VacancyId,
        string Content) : IRequest<string>;
}
