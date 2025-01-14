using MediatR;
using Microsoft.AspNetCore.Mvc;
using VacanciesService.Application.Interactions.Commands.AddInteraction;
using VacanciesService.Application.Interactions.Queries.GetUserInteractions;
using VacanciesService.Application.Interactions.Queries.GetVacancyInteractions;
using VacanciesService.Domain.Constants;
using VacanciesService.Domain.Models;
using VacanciesService.Presentation.Middleware.Authorization;

namespace VacanciesService.Presentation.Controllers
{
    [ApiController]
    [Route("/interactions")]
    public class InteractionsController : ControllerBase
    {
        private readonly ISender _sender;

        public InteractionsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        public async Task<ActionResult<Guid>> AddInteraction(AddInteractionCommand command, CancellationToken token)
        {
            return Ok(await _sender.Send(command, token));
        }

        [HttpGet]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        [Route("vacancies/{vacancyId}")]
        public async Task<ActionResult<List<VacancyInteraction>>> GetVacancyInteractions(Guid vacancyId, CancellationToken token)
        {
            return Ok(await _sender.Send(new GetVacancyInteractionsQuery(vacancyId), token));
        }

        [HttpGet]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        [Route("users/{userId}")]
        public async Task<ActionResult<List<VacancyInteraction>>> GetUserInteractions(Guid userId, CancellationToken token)
        {
            return Ok(await _sender.Send(new GetUserInteractionsQuery(userId), token));
        }
    }
}
