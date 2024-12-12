using MediatR;
using Microsoft.AspNetCore.Mvc;
using VacanciesService.Application.Vacancies.Commands.AddVacancyCommand;
using VacanciesService.Application.Vacancies.Commands.ArchiveVacancyCommand;
using VacanciesService.Application.Vacancies.Queries.GetBestVacanciesForResumeQuery;
using VacanciesService.Application.Vacancies.Queries.GetFilteredVacanciesQuery;
using VacanciesService.Application.Vacancies.Queries.GetVacanciesByCompanyQuery;
using VacanciesService.Application.Vacancies.Queries.GetVacancyQuery;
using VacanciesService.Application.VacanciesDetails.Commands.AddVacancyDetailsCommand;
using VacanciesService.Domain.Constants;
using VacanciesService.Domain.Filters.VacancyDetails;
using VacanciesService.Presentation.Middleware.Authorization;

namespace VacanciesService.Presentation.Controllers
{
    [ApiController]
    [Route("/vacancies")]
    public class VacanciesController : ControllerBase
    {
        private readonly ISender _sender;

        public VacanciesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        public async Task<IActionResult> Add(AddVacancyCommand command, CancellationToken token)
            => Ok(await _sender.Send(command, token));

        [HttpPost]
        [Route("archives/{id}")]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        public async Task<IActionResult> Archive(Guid id, CancellationToken token)
            => Ok(await _sender.Send(new ArchiveVacancyCommand(id), token));

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(Guid id, CancellationToken token)
            => Ok(await _sender.Send(new GetVacancyQuery(id), token));

        [HttpGet]
        [Route("companies/{companyId}")]
        public async Task<IActionResult> GetByCompany(Guid companyId, CancellationToken token)
            => Ok(await _sender.Send(new GetVacanciesByCompanyQuery(companyId), token));

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search(VacancyDetailsFilter filter, CancellationToken token)
            => Ok(await _sender.Send(new GetFilteredVacanciesQuery(filter), token));

        [HttpGet]
        [Route("recommendations/{resumeId}&pageNumber={pageNumber}&pageSize={pageSize}")]
        public async Task<IActionResult> GetBestVacanciesPageForResume(string resumeId, int pageNumber, int pageSize, CancellationToken token)
           => Ok(await _sender.Send(new GetBestVacanciesPageForResumeQuery(resumeId, pageNumber, pageSize), token));

        [HttpPost]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        [Route("details")]
        public async Task<IActionResult> AddDetails(AddVacancyDetailsCommand command, CancellationToken token)
            => Ok(await _sender.Send(command, token));
    }
}
