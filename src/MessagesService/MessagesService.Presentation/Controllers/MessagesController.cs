using MediatR;
using MessagesService.Application.Messages.Commands.EditMessage;
using MessagesService.Application.Messages.Commands.ReadMessage;
using MessagesService.Application.Messages.Commands.SendMessage;
using MessagesService.Application.Messages.Queries.GetApplicationMessages;
using MessagesService.Application.Messages.Queries.SearchApplicationMessages;
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

        [HttpPost]
        public async Task<ActionResult<string>> AddMessage(SendMessageCommand command, CancellationToken token = default)
        {
            return Ok(await _sender.Send(command, token));
        }

        [HttpPatch]
        public async Task<ActionResult> ReadMessage(ReadMessageCommand command, CancellationToken token = default)
        {
            await _sender.Send(command, token);

            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> EditMessage(EditMessageCommand command, CancellationToken token = default)
        {
            await _sender.Send(command, token);

            return Ok();
        }

        [HttpGet]
        [Route("{applicationId}&pageIndex={pageIndex}&pageSize={pageSize}")]
        public async Task<IActionResult> GetApplicationMessage(Guid applicationId, int pageIndex, int pageSize, CancellationToken token = default)
        {
            return Ok(await _sender.Send(new GetApplicationMessagesQuery(applicationId, pageIndex, pageSize), token));
        }

        [HttpGet]
        [Route("{applicationId}&content={content}")]
        public async Task<IActionResult> GetApplicationMessage(Guid applicationId, string content, CancellationToken token = default)
        {
            return Ok(await _sender.Send(new SearchApplicationMessagesQuery(applicationId, content), token));
        }
    }
}
