using AutoMapper;
using MediatR;
using MessagesService.Core.Models;
using MessagesService.DataAccess.Abstractions;
using Microsoft.Extensions.Logging;

namespace MessagesService.Application.Messages.Queries.GetChatMessages
{
    public class GetChatMessagesQueryHandler : IRequestHandler<GetChatMessagesQuery, List<Message>>
    {
        private readonly ILogger<GetChatMessagesQueryHandler> _logger;
        private readonly IMessagesRepository _messagesRepository;
        private readonly IMapper _mapper;

        public GetChatMessagesQueryHandler(
            ILogger<GetChatMessagesQueryHandler> logger,
            IMessagesRepository messagesRepository,
            IMapper mapper)
        {
            _logger = logger;
            _messagesRepository = messagesRepository;
            _mapper = mapper;
        }

        public async Task<List<Message>> Handle(GetChatMessagesQuery request, CancellationToken token)
        {
            _logger.LogInformation(
                "Start handling command {CommandName} for chat {ChatId}",
                request.GetType().Name,
                request.ChatId);

            var messagesEntities = await _messagesRepository.GetPageBy(
                msg => msg.ChatId,
                request.ChatId,
                request.PageIndex,
                request.PageSize,
                token);

            _logger.LogInformation(
                "Succesfully handled command {CommandName} for chat {ChatId}",
                request.GetType().Name,
                request.ChatId);

            return _mapper.Map<List<Message>>(messagesEntities);
        }
    }
}
