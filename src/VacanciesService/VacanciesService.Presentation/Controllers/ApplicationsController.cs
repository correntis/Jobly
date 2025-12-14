using MediatR;
using Microsoft.AspNetCore.Mvc;
using VacanciesService.Application.Applications.Commands.AddApplication;
using VacanciesService.Application.Applications.Commands.UpdateApplication;
using VacanciesService.Application.Applications.Queries.GetApplicationsByIds;
using VacanciesService.Application.Applications.Queries.GetApplicationsByUser;
using VacanciesService.Application.Applications.Queries.GetApplicationsByVacancy;
using VacanciesService.Application.Applications.Queries.GetApplicationByUserAndVacancy;
using VacanciesService.Domain.Constants;
using VacanciesService.Presentation.Middleware.Authorization;

namespace VacanciesService.Presentation.Controllers
{
    [ApiController]
    [Route("/applications")]
    public class ApplicationsController : ControllerBase
    {
        private readonly ISender _sender;

        public ApplicationsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<IActionResult> Add(AddApplicationCommand command, CancellationToken token)
        {
            return Ok(await _sender.Send(command, token));
        }

        [HttpPut]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        public async Task<IActionResult> Update(UpdateApplicationCommand command, CancellationToken token)
        {
            return Ok(await _sender.Send(command, token));
        }

        [HttpGet]
        [Route("users/{userId}&pageNumber={pageNumber}&pageSize={pageSize}")]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<IActionResult> GetByUser(Guid userId, int pageNumber, int pageSize, CancellationToken token)
        {
            return Ok(await _sender.Send(new GetApplicationsPageByUserQuery(userId, pageNumber, pageSize), token));
        }

        [HttpGet]
        [Route("vacancies/{vacancyId}&pageNumber={pageNumber}&pageSize={pageSize}")]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<IActionResult> GetByVacancy(Guid vacancyId, int pageNumber, int pageSize, CancellationToken token)
        {
            return Ok(await _sender.Send(new GetApplicationsPageByVacancyQuery(vacancyId, pageNumber, pageSize), token));
        }

        [HttpPost]
        [Route("ids")]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<IActionResult> GetByIds([FromBody] List<Guid> applicationsIds, CancellationToken token)
        {
            return Ok(await _sender.Send(new GetApplicationsByIdsQuery(applicationsIds), token));
        }

        [HttpGet]
        [Route("users/{userId}/vacancies/{vacancyId}")]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<IActionResult> GetByUserAndVacancy(Guid userId, Guid vacancyId, CancellationToken token)
        {
            var application = await _sender.Send(new GetApplicationByUserAndVacancyQuery(userId, vacancyId), token);
            return Ok(application);
        }
    }
}
