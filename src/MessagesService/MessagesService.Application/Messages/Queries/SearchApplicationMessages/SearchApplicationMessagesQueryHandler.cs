using AutoMapper;
using MediatR;
using MessagesService.Core.Models;
using MessagesService.DataAccess.Abstractions;
using Microsoft.Extensions.Logging;

namespace MessagesService.Application.Messages.Queries.SearchApplicationMessages
{
    public class SearchApplicationMessagesQueryHandler : IRequestHandler<SearchApplicationMessagesQuery, List<Message>>
    {
        private readonly ILogger<SearchApplicationMessagesQueryHandler> _logger;
        private readonly IMessagesRepository _messagesRepository;
        private readonly IMapper _mapper;

        public SearchApplicationMessagesQueryHandler(
            ILogger<SearchApplicationMessagesQueryHandler> logger,
            IMessagesRepository messagesRepository,
            IMapper mapper)
        {
            _logger = logger;
            _messagesRepository = messagesRepository;
            _mapper = mapper;
        }

        public async Task<List<Message>> Handle(SearchApplicationMessagesQuery request, CancellationToken token)
        {
            _logger.LogInformation(
                "Start handling command {CommandName} for application {ApplicationId}",
                request.GetType().Name,
                request.ApplicationId);

            var messagesEntities = await _messagesRepository.SearchContentForApplication(
                request.ApplicationId,
                request.Content,
                token);

            _logger.LogInformation(
                "Succesfully handled command {CommandName} for application {ApplicationId}",
                request.GetType().Name,
                request.ApplicationId);

            return _mapper.Map<List<Message>>(messagesEntities);
        }
    }
}
