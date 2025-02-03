using AutoMapper;
using MediatR;
using MessagesService.Core.Models;
using MessagesService.DataAccess.Abstractions;
using Microsoft.Extensions.Logging;

namespace MessagesService.Application.Chats.Queries.GetChatByApplication
{
    public class GetChatByApplicationQueryHandler : IRequestHandler<GetChatByApplicationQuery, Chat>
    {
        private readonly ILogger<GetChatByApplicationQueryHandler> _logger;
        private readonly IChatsRepository _chatsRepository;
        private readonly IMapper _mapper;

        public GetChatByApplicationQueryHandler(
            ILogger<GetChatByApplicationQueryHandler> logger,
            IChatsRepository chatsRepository,
            IMapper mapper)
        {
            _logger = logger;
            _chatsRepository = chatsRepository;
            _mapper = mapper;
        }

        public async Task<Chat> Handle(GetChatByApplicationQuery request, CancellationToken token)
        {
            _logger.LogInformation(
             "Start handling command {CommandName} for application {ApplicationId}",
             request.GetType().Name,
             request.ApplicationId);

            var chatEntity = await _chatsRepository.GetOneByAsync(
                chat => chat.ApplicationId,
                request.ApplicationId,
                token);

            _logger.LogInformation(
                "Succesfully handled command {CommandName} for company {CompanyId}",
                request.GetType().Name,
                request.ApplicationId);

            return _mapper.Map<Chat>(chatEntity);
        }
    }
}
