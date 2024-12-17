using AutoMapper;
using MediatR;
using MessagesService.Core.Models;
using MessagesService.DataAccess.Abstractions;
using Microsoft.Extensions.Logging;

namespace MessagesService.Application.Messages.Queries.GetApplicationMessages
{
    public class GetApplicationMessagesQueryHandler : IRequestHandler<GetApplicationMessagesQuery, List<Message>>
    {
        private readonly ILogger<GetApplicationMessagesQueryHandler> _logger;
        private readonly IMessagesRepository _messagesRepository;
        private readonly IMapper _mapper;

        public GetApplicationMessagesQueryHandler(
            ILogger<GetApplicationMessagesQueryHandler> logger,
            IMessagesRepository messagesRepository,
            IMapper mapper)
        {
            _logger = logger;
            _messagesRepository = messagesRepository;
            _mapper = mapper;
        }

        public async Task<List<Message>> Handle(GetApplicationMessagesQuery request, CancellationToken token)
        {
            _logger.LogInformation(
                "Start handling command {CommandName} for application {ApplicationId}",
                request.GetType().Name,
                request.ApplicationId);

            var messagesEntities = await _messagesRepository.GetPageBy(
                msg => msg.ApplicationId,
                request.ApplicationId,
                request.PageIndex,
                request.PageSize,
                token);
            
            _logger.LogInformation(
                "Succesfully handled command {CommandName} for application {ApplicationId}",
                request.GetType().Name,
                request.ApplicationId);

            return _mapper.Map<List<Message>>(messagesEntities);
        }
    }
}
