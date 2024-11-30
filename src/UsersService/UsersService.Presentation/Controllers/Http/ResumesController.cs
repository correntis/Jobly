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
using UsersService.Domain.Models;
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
        public async Task<ActionResult<string>> Add(AddResumeCommand addResumeCommand, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(addResumeCommand, cancellationToken));

        [HttpPut]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult<string>> Update(UpdateResumeCommand updateResumeCommand, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(updateResumeCommand, cancellationToken));

        [HttpPut("certifications")]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult<string>> UpdateCertification(UpdateCertificationCommand updateCertificationCommand, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(updateCertificationCommand, cancellationToken));

        [HttpPut("educations")]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult<string>> UpdateEducation(UpdateEducationCommand updateEducationCommand, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(updateEducationCommand, cancellationToken));

        [HttpPut("experiences")]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult<string>> UpdateExperiences(UpdateJobExperienceCommand updateJobExperienceCommand, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(updateJobExperienceCommand, cancellationToken));

        [HttpPut("languages")]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult<string>> UpdateLanguages(UpdateLanguageCommand updateLanguageCommand, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(updateLanguageCommand, cancellationToken));

        [HttpPut("projects")]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult<string>> UpdateProjects(UpdateProjectCommand updateProjectCommand, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(updateProjectCommand, cancellationToken));

        [HttpDelete("{id}")]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult<string>> Delete(string id, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new DeleteResumeCommand(id), cancellationToken));

        [HttpGet("{id}")]
        public async Task<ActionResult<Resume>> Get(string id, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new GetResumeQuery(id), cancellationToken));

        [HttpGet("users/{userId}")]
        public async Task<ActionResult<Resume>> GetByUser(Guid userId, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new GetResumeByUserQuery(userId), cancellationToken));
    }
}
