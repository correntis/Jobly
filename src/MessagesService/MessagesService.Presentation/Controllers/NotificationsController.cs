using MediatR;
using MessagesService.Application.Notifications.Commands.ViewNotification;
using MessagesService.Application.Notifications.Queries.GetRecipientNotifications;
using MessagesService.Core.Models;
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
        [Route("{recipientId}/unreaded")]
        public async Task<ActionResult<List<Notification>>> GetRecipientNotifications(Guid recipientId, CancellationToken token)
        {
            return Ok(await _sender.Send(new GetRecipientUnreadedNotificationsQuery(recipientId), token));
        }
    }
}
