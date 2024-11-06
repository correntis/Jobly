using MediatR;
using Microsoft.AspNetCore.Mvc;
using UsersService.Application.Resumes.Commands.AddResumeCommand;
using UsersService.Application.Resumes.Commands.DeleteResumeCommand;
using UsersService.Application.Resumes.Commands.UpdateCertificationCommand;
using UsersService.Application.Resumes.Commands.UpdateEducationCommand;
using UsersService.Application.Resumes.Commands.UpdateJobExperienceCommand;
using UsersService.Application.Resumes.Commands.UpdateLanguageCommand;
using UsersService.Application.Resumes.Commands.UpdateProjectCommand;
using UsersService.Application.Resumes.Commands.UpdateResumeCommand;
using UsersService.Application.Resumes.Queries.GetResumeByUser;
using UsersService.Application.Resumes.Queries.GetResumeQuery;
using UsersService.Domain.Constants;
using UsersService.Presentation.Middleware.Authentication;

namespace UsersService.Presentation.Controllers.Http
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
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<IActionResult> Add(AddResumeCommand addResumeCommand, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(addResumeCommand, cancellationToken));

        [HttpPut]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<IActionResult> Update(UpdateResumeCommand updateResumeCommand, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(updateResumeCommand, cancellationToken));

        [HttpPut("certifications")]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<IActionResult> UpdateCertification(UpdateCertificationCommand updateCertificationCommand, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(updateCertificationCommand, cancellationToken));

        [HttpPut("educations")]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<IActionResult> UpdateEducation(UpdateEducationCommand updateEducationCommand, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(updateEducationCommand, cancellationToken));

        [HttpPut("experiences")]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<IActionResult> UpdateExperiences(UpdateJobExperienceCommand updateJobExperienceCommand, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(updateJobExperienceCommand, cancellationToken));

        [HttpPut("languages")]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<IActionResult> UpdateLanguages(UpdateLanguageCommand updateLanguageCommand, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(updateLanguageCommand, cancellationToken));

        [HttpPut("projects")]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<IActionResult> UpdateProjects(UpdateProjectCommand updateProjectCommand, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(updateProjectCommand, cancellationToken));

        [HttpDelete("{id}")]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
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
