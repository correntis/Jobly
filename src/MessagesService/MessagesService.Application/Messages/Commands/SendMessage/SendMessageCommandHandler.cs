using AutoMapper;
using MediatR;
using MessagesService.DataAccess.Abstractions;
using MessagesService.DataAccess.Entities;
using Microsoft.Extensions.Logging;

namespace MessagesService.Application.Messages.Commands.SendMessage
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, string>
    {
        private readonly ILogger<SendMessageCommandHandler> _logger;
        private readonly IMessagesRepository _messagesRepository;
        private readonly IMapper _mapper;

        public SendMessageCommandHandler(
            ILogger<SendMessageCommandHandler> logger,
            IMessagesRepository messagesRepository,
            IMapper mapper)
        {
            _logger = logger;
            _messagesRepository = messagesRepository;
            _mapper = mapper;
        }

        public async Task<string> Handle(SendMessageCommand request, CancellationToken token)
        {
            _logger.LogInformation(
                "Start handling command {CommandName} for application {ApplicationId} and sender {SenderId}",
                request.GetType().Name,
                request.ApplicationId,
                request.SenderId);

            var message = _mapper.Map<MessageEntity>(request);

            await _messagesRepository.AddAsync(message, token);

            _logger.LogInformation(
                "Succesfully handled command {CommandName} for application {ApplicationId} and sender {SenderId}",
                request.GetType().Name,
                request.ApplicationId,
                request.SenderId);

            return message.Id;
        }
    }
}
