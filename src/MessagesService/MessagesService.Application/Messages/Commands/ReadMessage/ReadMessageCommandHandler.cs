using MediatR;
using MessagesService.DataAccess.Abstractions;
using Microsoft.Extensions.Logging;

namespace MessagesService.Application.Messages.Commands.ReadMessage
{
    public class ReadMessageCommandHandler : IRequestHandler<ReadMessageCommand>
    {
        private readonly ILogger<ReadMessageCommandHandler> _logger;
        private readonly IMessagesRepository _messagesRepository;

        public ReadMessageCommandHandler(
            ILogger<ReadMessageCommandHandler> logger,
            IMessagesRepository messagesRepository)
        {
            _logger = logger;
            _messagesRepository = messagesRepository;
        }

        public async Task Handle(ReadMessageCommand request, CancellationToken token)
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
                msg => msg.IsRead,
                true,
                token);

            _logger.LogInformation(
                "Succesfully handled command {CommandName} for message {MessageId}",
                request.GetType().Name,
                request.MessageId);

        }
    }
}
