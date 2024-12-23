using MediatR;
using MessagesService.Application.Messages.Queries.GetApplicationMessages;
using MessagesService.Application.Messages.Queries.SearchApplicationMessages;
using MessagesService.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace MessagesService.Presentation.Controllers
{
    [ApiController]
    [Route("/messages")]
    public class MessagesController : ControllerBase
    {
        private readonly ISender _sender;

        public MessagesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        [Route("{applicationId}&pageIndex={pageIndex}&pageSize={pageSize}")]
        public async Task<ActionResult<List<Message>>> GetApplicationMessages(Guid applicationId, int pageIndex, int pageSize, CancellationToken token = default)
        {
            return Ok(await _sender.Send(new GetApplicationMessagesQuery(applicationId, pageIndex, pageSize), token));
        }

        [HttpGet]
        [Route("{applicationId}&content={content}")]
        public async Task<ActionResult<List<Message>>> SearchApplicationMessages(Guid applicationId, string content, CancellationToken token = default)
        {
            return Ok(await _sender.Send(new SearchChatMessagesQuery(applicationId, content), token));
        }
    }
}
