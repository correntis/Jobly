using MediatR;
using Microsoft.AspNetCore.Mvc;
using VacanciesService.Application.Interactions.Commands.AddInteraction;
using VacanciesService.Application.Interactions.Queries.GetUserForVacancyInteractions;
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
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        [Route("vacancies/{vacancyId}")]
        public async Task<ActionResult<List<VacancyInteraction>>> GetVacancyInteractions(Guid vacancyId, CancellationToken token)
        {
            return Ok(await _sender.Send(new GetVacancyInteractionsQuery(vacancyId), token));
        }

        [HttpGet]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        [Route("users/{userId}")]
        public async Task<ActionResult<List<VacancyInteraction>>> GetUserInteractions(Guid userId, CancellationToken token)
        {
            return Ok(await _sender.Send(new GetUserInteractionsQuery(userId), token));
        }

        [HttpGet]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        [Route("users/{userId}/vacancies/{vacancyId}")]
        public async Task<ActionResult<VacancyInteraction>> GetUserForVacancyInteractions(Guid userId, Guid vacancyId, CancellationToken token)
        {
            var interaction = await _sender.Send(new GetUserForVacancyInteractionQuery(userId, vacancyId), token);

            return Ok(interaction);
        }
    }
}
