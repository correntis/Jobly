using AutoMapper;
using MediatR;
using MessagesService.Core.Models;
using MessagesService.DataAccess.Abstractions;
using Microsoft.Extensions.Logging;

namespace MessagesService.Application.Messages.Queries.SearchChatMessages
{
    public class SearchChatMessagesQueryHandler : IRequestHandler<SearchChatMessagesQuery, List<Message>>
    {
        private readonly ILogger<SearchChatMessagesQueryHandler> _logger;
        private readonly IMessagesRepository _messagesRepository;
        private readonly IMapper _mapper;

        public SearchChatMessagesQueryHandler(
            ILogger<SearchChatMessagesQueryHandler> logger,
            IMessagesRepository messagesRepository,
            IMapper mapper)
        {
            _logger = logger;
            _messagesRepository = messagesRepository;
            _mapper = mapper;
        }

        public async Task<List<Message>> Handle(SearchChatMessagesQuery request, CancellationToken token)
        {
            _logger.LogInformation(
                "Start handling command {CommandName} for chat {ChatId}",
                request.GetType().Name,
                request.ChatId);

            var messagesEntities = await _messagesRepository.SearchChatContent(
                request.ChatId,
                request.Content,
                token);

            _logger.LogInformation(
                "Succesfully handled command {CommandName} for chat {ChatId}",
                request.GetType().Name,
                request.ChatId);

            return _mapper.Map<List<Message>>(messagesEntities);
        }
    }
}
