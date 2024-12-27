using MediatR;
using MessagesService.Application.Messages.Queries.GetChatMessages;
using MessagesService.Application.Messages.Queries.SearchChatMessages;
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
        [Route("{chatId}&pageIndex={pageIndex}&pageSize={pageSize}")]
        public async Task<ActionResult<List<Message>>> GetApplicationMessages(string chatId, int pageIndex, int pageSize, CancellationToken token = default)
        {
            return Ok(await _sender.Send(new GetChatMessagesQuery(chatId, pageIndex, pageSize), token));
        }

        [HttpGet]
        [Route("{chatId}&content={content}")]
        public async Task<ActionResult<List<Message>>> SearchApplicationMessages(string chatId, string content, CancellationToken token = default)
        {
            return Ok(await _sender.Send(new SearchChatMessagesQuery(chatId, content), token));
        }
    }
}
