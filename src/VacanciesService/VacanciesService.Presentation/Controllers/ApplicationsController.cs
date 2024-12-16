using MediatR;
using Microsoft.AspNetCore.Mvc;
using VacanciesService.Application.Applications.Commands.AddApplicationCommand;
using VacanciesService.Application.Applications.Commands.UpdateApplicationCommand;
using VacanciesService.Application.Applications.Queries.GetApplicationsByUserQuery;
using VacanciesService.Application.Applications.Queries.GetApplicationsByVacancyQuery;
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
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        [Route("users/{userId}&pageNumber={pageNumber}&pageSize={pageSize}")]
        public async Task<IActionResult> GetByUser(Guid userId, int pageNumber, int pageSize, CancellationToken token)
        {
            return Ok(await _sender.Send(new GetApplicationsPageByUserQuery(userId, pageNumber, pageSize), token));
        }

        [HttpGet]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        [Route("vacancies/{vacancyId}&pageNumber={pageNumber}&pageSize={pageSize}")]
        public async Task<IActionResult> GetByVacancy(Guid vacancyId, int pageNumber, int pageSize, CancellationToken token)
        {
            return Ok(await _sender.Send(new GetApplicationsPageByVacancyQuery(vacancyId, pageNumber, pageSize), token));
        }
    }
}
