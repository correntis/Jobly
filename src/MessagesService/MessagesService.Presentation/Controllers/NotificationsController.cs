using MediatR;
using MessagesService.Application.Notifications.Commands.ViewNotification;
using MessagesService.Application.Notifications.Queries.GetRecipientNotifications;
using Microsoft.AspNetCore.Mvc;

namespace MessagesService.Presentation.Controllers
{
    [ApiController]
    [Route("/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly ISender _sender;

        public NotificationsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPatch]
        public async Task<ActionResult> ViewNotification(ViewNotificationCommand command, CancellationToken token)
        {
            await _sender.Send(command, token);

            return Ok();
        }

        [HttpGet]
        [Route("{recipientId}&pageIndex={pageIndex}&pageSize={pageSize}")]
        public async Task<ActionResult> GetRecipientNotifications(Guid recipientId, int pageIndex, int pageSize, CancellationToken token)
        {
            await _sender.Send(new GetRecipientNotificationsQuery(recipientId, pageIndex, pageSize), token);

            return Ok();
        }
    }
}
