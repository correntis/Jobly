using MediatR;
using Microsoft.AspNetCore.Mvc;
using UsersService.Application.Resumes.Commands.AddResumeCommand;
using UsersService.Application.Resumes.Commands.DeleteResumeCommand;
using UsersService.Application.Resumes.Commands.UpdateResumeCommand;
using UsersService.Application.Resumes.Queries.GetResumeByUser;
using UsersService.Application.Resumes.Queries.GetResumeQuery;

namespace UsersService.API.Controllers
{
    [ApiController]
    [Route("/resumes")]
    public class ResumesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ResumesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddResumeCommand addResumeCommand, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(addResumeCommand, cancellationToken));

        [HttpPut]
        public async Task<IActionResult> Update(UpdateResumeCommand updateResumeCommand, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(updateResumeCommand, cancellationToken));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new DeleteResumeCommand(id), cancellationToken));

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new GetResumeQuery(id), cancellationToken));

        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetByUser(int userId, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new GetResumeByUserQuery(userId), cancellationToken));
    }
}
