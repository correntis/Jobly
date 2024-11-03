using MediatR;
using Microsoft.AspNetCore.Mvc;
using UsersService.Application.Users.Commands.AddUserCommand;
using UsersService.Application.Users.Commands.DeleteUserCommand;
using UsersService.Application.Users.Commands.UpdateUserCommand;
using UsersService.Application.Users.Queries.GetUserQuery;

namespace UsersService.API.Controllers
{
    [ApiController]
    [Route("/users")]
    public class TestController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddUserCommand addUserCommand, CancellationToken cancellationToken)
        {
            await _mediator.Send(addUserCommand, cancellationToken);

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateUserCommand updateUserCommand, CancellationToken cancellationToken)
        {
            await _mediator.Send(updateUserCommand, cancellationToken);

            return Ok();
        }

        [HttpDelete("/{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteUserCommand(id), cancellationToken);

            return Ok();
        }

        [HttpGet("/{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetUserQuery(id), cancellationToken));
        }
    }
}
