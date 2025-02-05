using MediatR;
using Microsoft.AspNetCore.Mvc;
using UsersService.Application.Companies.Commands.ViewResume;
using UsersService.Application.Resumes.Commands.AddResume;
using UsersService.Application.Resumes.Commands.DeleteResume;
using UsersService.Application.Resumes.Commands.UpdateCertification;
using UsersService.Application.Resumes.Commands.UpdateEducation;
using UsersService.Application.Resumes.Commands.UpdateJobExperience;
using UsersService.Application.Resumes.Commands.UpdateLanguage;
using UsersService.Application.Resumes.Commands.UpdateProject;
using UsersService.Application.Resumes.Commands.UpdateResume;
using UsersService.Application.Resumes.Queries.GetResume;
using UsersService.Application.Resumes.Queries.GetResumeByUser;
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
        {
            var resumeId = await _mediator.Send(addResumeCommand, cancellationToken);

            return Ok(resumeId);
        }

        [HttpPut]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult<string>> Update(UpdateResumeCommand updateResumeCommand, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(updateResumeCommand, cancellationToken));
        }

        [HttpPost]
        [Route("views")]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        public async Task<ActionResult<Guid>> ViewResume(ViewResumeCommand viewResumeCommand, CancellationToken cancellationToken)
        {
            await _mediator.Send(viewResumeCommand, cancellationToken);

            return Ok();
        }

        [HttpPut("certifications")]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult<string>> UpdateCertification(UpdateCertificationCommand updateCertificationCommand, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(updateCertificationCommand, cancellationToken));
        }

        [HttpPut("educations")]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult<string>> UpdateEducation(UpdateEducationCommand updateEducationCommand, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(updateEducationCommand, cancellationToken));
        }

        [HttpPut("experiences")]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult<string>> UpdateExperiences(UpdateJobExperienceCommand updateJobExperienceCommand, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(updateJobExperienceCommand, cancellationToken));
        }

        [HttpPut("languages")]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult<string>> UpdateLanguages(UpdateLanguageCommand updateLanguageCommand, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(updateLanguageCommand, cancellationToken));
        }

        [HttpPut("projects")]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult<string>> UpdateProjects(UpdateProjectCommand updateProjectCommand, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(updateProjectCommand, cancellationToken));
        }

        [HttpDelete("{id}")]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult<string>> Delete(string id, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new DeleteResumeCommand(id), cancellationToken));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Resume>> Get(string id, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetResumeQuery(id), cancellationToken));
        }

        [HttpGet("users/{userId}")]
        public async Task<ActionResult<Resume>> GetByUser(Guid userId, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetResumeByUserQuery(userId), cancellationToken));
        }
    }
}
