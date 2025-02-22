using AutoMapper;
using MediatR;
using MessagesService.Core.Models;
using MessagesService.DataAccess.Abstractions;
using Microsoft.Extensions.Logging;

namespace MessagesService.Application.Chats.Queries.GetChatsPageByCompany
{
    public class GetChatsPageByCompanyQueryHandler : IRequestHandler<GetChatsPageByCompanyQuery, List<Chat>>
    {
        private readonly ILogger<GetChatsPageByCompanyQueryHandler> _logger;
        private readonly IChatsRepository _chatsRepository;
        private readonly IMapper _mapper;

        public GetChatsPageByCompanyQueryHandler(
            ILogger<GetChatsPageByCompanyQueryHandler> logger,
            IChatsRepository chatsRepository,
            IMapper mapper)
        {
            _logger = logger;
            _chatsRepository = chatsRepository;
            _mapper = mapper;
        }

        public async Task<List<Chat>> Handle(GetChatsPageByCompanyQuery request, CancellationToken token)
        {
            _logger.LogInformation(
             "Start handling command {CommandName} for company {CompanyId}",
             request.GetType().Name,
             request.CompanyId);

            var chatsEntities = await _chatsRepository.GetPageByAsync(
                chat => chat.CompanyId,
                request.CompanyId,
                request.PageIndex,
                request.PageSize,
                token);

            _logger.LogInformation(
                "Succesfully handled command {CommandName} for company {CompanyId}",
                request.GetType().Name,
                request.CompanyId);

            return _mapper.Map<List<Chat>>(chatsEntities);
        }
    }
}
