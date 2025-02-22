using MediatR;
using MessagesService.Application.Messages.Queries.GetChatMessages;
using MessagesService.Application.Messages.Queries.SearchChatMessages;
using MessagesService.Core.Constants;
using MessagesService.Core.Models;
using MessagesService.Presentation.Middleware.Authorization;
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
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult<List<Message>>> GetApplicationMessages(string chatId, int pageIndex, int pageSize, CancellationToken token = default)
        {
            return Ok(await _sender.Send(new GetChatMessagesQuery(chatId, pageIndex, pageSize), token));
        }

        [HttpGet]
        [Route("{chatId}&content={content}")]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult<List<Message>>> SearchApplicationMessages(string chatId, string content, CancellationToken token = default)
        {
            return Ok(await _sender.Send(new SearchChatMessagesQuery(chatId, content), token));
        }
    }
}
