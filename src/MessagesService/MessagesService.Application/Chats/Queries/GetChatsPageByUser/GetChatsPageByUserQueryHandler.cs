using AutoMapper;
using MediatR;
using MessagesService.Application.Chats.Queries.GetPageByUser;
using MessagesService.Core.Models;
using MessagesService.DataAccess.Abstractions;
using Microsoft.Extensions.Logging;

namespace MessagesService.Application.Chats.Queries.GetChatsPageByUser
{
    public class GetChatsPageByUserQueryHandler : IRequestHandler<GetChatsPageByUserQuery, List<Chat>>
    {
        private readonly ILogger<GetChatsPageByUserQueryHandler> _logger;
        private readonly IChatsRepository _chatsRepository;
        private readonly IMapper _mapper;

        public GetChatsPageByUserQueryHandler(
            ILogger<GetChatsPageByUserQueryHandler> logger,
            IChatsRepository chatsRepository,
            IMapper mapper)
        {
            _logger = logger;
            _chatsRepository = chatsRepository;
            _mapper = mapper;
        }

        public async Task<List<Chat>> Handle(GetChatsPageByUserQuery request, CancellationToken token)
        {
            _logger.LogInformation(
             "Start handling command {CommandName} for user {UserId}",
             request.GetType().Name,
             request.UserId);

            var chatsEntities = await _chatsRepository.GetPageByAsync(
                chat => chat.UserId,
                request.UserId,
                request.PageIndex,
                request.PageSize,
                token);

            _logger.LogInformation(
                "Succesfully handled command {CommandName} for user {UserId}",
                request.GetType().Name,
                request.UserId);

            return _mapper.Map<List<Chat>>(chatsEntities);
        }
    }
}
