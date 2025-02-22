using MediatR;
using Microsoft.AspNetCore.Mvc;
using VacanciesService.Application.Vacancies.Commands.AddVacancy;
using VacanciesService.Application.Vacancies.Commands.ArchiveVacancy;
using VacanciesService.Application.Vacancies.Queries.GetBestVacanciesForResume;
using VacanciesService.Application.Vacancies.Queries.GetFilteredVacancies;
using VacanciesService.Application.Vacancies.Queries.GetVacanciesByCompany;
using VacanciesService.Application.Vacancies.Queries.GetVacancy;
using VacanciesService.Application.VacanciesDetails.Commands.AddVacancyDetails;
using VacanciesService.Domain.Constants;
using VacanciesService.Domain.Filters.VacancyDetails;
using VacanciesService.Domain.Models;
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
        public async Task<ActionResult<Guid>> Add(AddVacancyCommand command, CancellationToken token)
        {
            return Ok(await _sender.Send(command, token));
        }


        [HttpPost]
        [Route("details")]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        public async Task<ActionResult<string>> AddDetails(AddVacancyDetailsCommand command, CancellationToken token)
        {
            return Ok(await _sender.Send(command, token));
        }

        [HttpPost]
        [Route("archives/{id}")]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        public async Task<ActionResult<Guid>> Archive(Guid id, CancellationToken token)
        {
            return Ok(await _sender.Send(new ArchiveVacancyCommand(id), token));
        }

        [HttpGet]
        [Route("{id}")]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult<Vacancy>> Get(Guid id, CancellationToken token)
        {
            return Ok(await _sender.Send(new GetVacancyQuery(id), token));
        }

        [HttpGet]
        [Route("companies/{companyId}")]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult<List<Vacancy>>> GetByCompany(Guid companyId, CancellationToken token)
        {
            return Ok(await _sender.Send(new GetVacanciesByCompanyQuery(companyId), token));
        }

        [HttpPost]
        [Route("search")]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult<List<Vacancy>>> Search(VacancyDetailsFilter filter, CancellationToken token)
        {
            return Ok(await _sender.Send(new GetFilteredVacanciesQuery(filter), token));
        }

        [HttpGet]
        [Route("recommendations/{resumeId}&pageNumber={pageNumber}&pageSize={pageSize}")]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult<List<Vacancy>>> GetBestVacanciesPageForResume(string resumeId, int pageNumber, int pageSize, CancellationToken token)
        {
            return Ok(await _sender.Send(new GetBestVacanciesPageForResumeQuery(resumeId, pageNumber, pageSize), token));
        }
    }
}
