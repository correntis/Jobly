using MediatR;
using Microsoft.AspNetCore.Mvc;
using Polly;
using VacanciesService.Application.Vacancies.Commands.AddVacancy;
using VacanciesService.Application.Vacancies.Commands.ArchiveVacancy;
using VacanciesService.Application.Vacancies.Queries.GetBestVacanciesForResume;
using VacanciesService.Application.Vacancies.Queries.GetDistinctRequirements;
using VacanciesService.Application.Vacancies.Queries.GetDistinctSkills;
using VacanciesService.Application.Vacancies.Queries.GetDistinctTechnologies;
using VacanciesService.Application.Vacancies.Queries.GetFilteredVacancies;
using VacanciesService.Application.Vacancies.Queries.GetVacanciesByCompany;
using VacanciesService.Application.Vacancies.Queries.GetVacancy;
using VacanciesService.Application.VacanciesDetails.Commands.AddVacancyDetails;
using VacanciesService.Domain.Constants;
using VacanciesService.Domain.Filters.VacancyDetails;
using VacanciesService.Domain.Models;
using VacanciesService.Presentation.Helpers;
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
            var userId = JwtTokenHelper.GetUserIdFromHttpContext(HttpContext);
            
            return Ok(await _sender.Send(new GetFilteredVacanciesQuery(filter, userId), token));
        }

        [HttpGet]
        [Route("recommendations/{resumeId}&pageNumber={pageNumber}&pageSize={pageSize}")]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult<List<Vacancy>>> GetBestVacanciesPageForResume(string resumeId, int pageNumber, int pageSize, CancellationToken token)
        {
            return Ok(await _sender.Send(new GetBestVacanciesPageForResumeQuery(resumeId, pageNumber, pageSize), token));
        }

        [HttpGet]
        [Route("interactions/{userId}&type={interactionType}&pageNumber={pageNumber}&pageSize={pageSize}")]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult<List<Vacancy>>> GetVacanciesByInteraction(Guid userId, int interactionType, int pageNumber, int pageSize, CancellationToken token)
        {
            var interactionTypeEnum = (Domain.Enums.InteractionType)interactionType;
            return Ok(await _sender.Send(new Application.Vacancies.Queries.GetVacanciesByInteraction.GetVacanciesByInteractionQuery(userId, interactionTypeEnum, pageNumber, pageSize), token));
        }

        [HttpGet]
        [Route("distinct/requirements")]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult<List<string>>> GetDistinctRequirements(CancellationToken token)
        {
            return Ok(await _sender.Send(new GetDistinctRequirementsQuery(), token));
        }

        [HttpGet]
        [Route("distinct/skills")]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult<List<string>>> GetDistinctSkills(CancellationToken token)
        {
            return Ok(await _sender.Send(new GetDistinctSkillsQuery(), token));
        }

        [HttpGet]
        [Route("distinct/technologies")]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult<List<string>>> GetDistinctTechnologies(CancellationToken token)
        {
            return Ok(await _sender.Send(new GetDistinctTechnologiesQuery(), token));
        }
    }
}
