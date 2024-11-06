using MediatR;
using Microsoft.AspNetCore.Mvc;
using UsersService.API.Middleware.Authentication;
using UsersService.Application.Users.Commands.DeleteUserCommand;
using UsersService.Application.Users.Commands.UpdateUserCommand;
using UsersService.Application.Users.Queries.GetUserQuery;
using UsersService.Domain.Constants;

namespace UsersService.API.Controllers.Http
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
        public async Task<IActionResult> Update(UpdateUserCommand updateUserCommand, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(updateUserCommand, cancellationToken));

        [HttpDelete("{id}")]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new DeleteUserCommand(id), cancellationToken));

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new GetUserQuery(id), cancellationToken));
    }
}
