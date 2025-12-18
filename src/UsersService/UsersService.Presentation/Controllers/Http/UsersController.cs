using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using UsersService.Application.Users.Commands.DeleteUser;
using UsersService.Application.Users.Commands.UpdateTelegramChatId;
using UsersService.Application.Users.Commands.UpdateUser;
using UsersService.Application.Users.Queries.GetUser;
using UsersService.Domain.Constants;
using UsersService.Domain.Models;
using UsersService.Presentation.Middleware.Authentication;

namespace UsersService.Presentation.Controllers.Http
{
    [ApiController]
    [Route("/users")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPut]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        public async Task<ActionResult<Guid>> Update(UpdateUserCommand updateUserCommand, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(updateUserCommand, cancellationToken));
        }

        [HttpDelete("{id}")]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        public async Task<ActionResult<Guid>> Delete(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new DeleteUserCommand(id), cancellationToken));
        }

        [HttpGet("{id}")]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        public async Task<ActionResult<User>> Get(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetUserQuery(id), cancellationToken));
        }

        [HttpPost("{id}/telegram/connect")]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        public ActionResult<string> GetTelegramConnectionLink(Guid id, [FromServices] IConfiguration configuration)
        {
            // Получаем username бота из конфигурации или используем значение по умолчанию
            var botUsername = configuration["Telegram:BotUsername"] ?? "jobly_app_bot";
            var link = $"https://t.me/{botUsername}?start={id}";
            return Ok(link);
        }

    }
}
