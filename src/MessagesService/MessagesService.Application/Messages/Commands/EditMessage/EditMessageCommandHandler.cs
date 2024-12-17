using MediatR;
using MessagesService.DataAccess.Abstractions;
using Microsoft.Extensions.Logging;

namespace MessagesService.Application.Messages.Commands.EditMessage
{
    public class EditMessageCommandHandler : IRequestHandler<EditMessageCommand>
    {
        private readonly ILogger<EditMessageCommandHandler> _logger;
        private readonly IMessagesRepository _messagesRepository;

        public EditMessageCommandHandler(
            ILogger<EditMessageCommandHandler> logger,
            IMessagesRepository messagesRepository)
        {
            _logger = logger;
            _messagesRepository = messagesRepository;
        }

        public async Task Handle(EditMessageCommand request, CancellationToken token)
        {
            _logger.LogInformation(
                "Start handling command {CommandName} for message {MessageId}",
                request.GetType().Name,
                request.MessageId);

            var messageEntity = await _messagesRepository.GetOneBy(msg => msg.Id, request.MessageId, token);

            if(messageEntity is null)
            {
                // Add processing errors with ResultType 
                throw new NotImplementedException();
            }

            await _messagesRepository.SetByIdAsync(
                request.MessageId,
                msg => msg.Content,
                request.Content,
                token);

            _logger.LogInformation(
                "Succesfully handled command {CommandName} for message {MessageId}",
                request.GetType().Name,
                request.MessageId);

        }
    }
}
