using MediatR;
using MessagesService.Application.Notifications.Commands.ViewNotification;
using MessagesService.Application.Notifications.Queries.GetRecipientNotifications;
using MessagesService.Core.Constants;
using MessagesService.Core.Models;
using MessagesService.Presentation.Middleware.Authorization;
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
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult> ViewNotification(ViewNotificationCommand command, CancellationToken token)
        {
            await _sender.Send(command, token);

            return Ok();
        }

        [HttpGet]
        [Route("{recipientId}/unreaded")]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult<List<Notification>>> GetRecipientNotifications(Guid recipientId, CancellationToken token)
        {
            return Ok(await _sender.Send(new GetRecipientUnreadedNotificationsQuery(recipientId), token));
        }
    }
}
