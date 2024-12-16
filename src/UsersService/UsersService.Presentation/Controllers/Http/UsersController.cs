using MediatR;
using Microsoft.AspNetCore.Mvc;
using UsersService.Application.Users.Commands.DeleteUserCommand;
using UsersService.Application.Users.Commands.UpdateUserCommand;
using UsersService.Application.Users.Queries.GetUserQuery;
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
        public async Task<ActionResult<Guid>> Update(UpdateUserCommand updateUserCommand, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(updateUserCommand, cancellationToken));
        }

        [HttpDelete("{id}")]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult<Guid>> Delete(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new DeleteUserCommand(id), cancellationToken));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Get(Guid id, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetUserQuery(id), cancellationToken));
        }
    }
}
