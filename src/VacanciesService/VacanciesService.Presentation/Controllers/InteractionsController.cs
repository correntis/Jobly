using MediatR;
using Microsoft.AspNetCore.Mvc;
using VacanciesService.Application.Interactions.Commands.AddInteractionCommand;
using VacanciesService.Application.Interactions.Queries.GetUserInteractionsQuery;
using VacanciesService.Application.Interactions.Queries.GetVacancyInteractionsQuery;
using VacanciesService.Domain.Constants;
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
        public async Task<IActionResult> AddInteraction(AddInteractionCommand command, CancellationToken token)
            => Ok(await _sender.Send(command, token));

        [HttpGet]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        [Route("vacancies/{vacancyId}")]
        public async Task<IActionResult> GetVacancyInteractions(Guid vacancyId, CancellationToken token)
            => Ok(await _sender.Send(new GetVacancyInteractionsQuery(vacancyId), token));

        [HttpGet]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        [Route("users/{userId}")]
        public async Task<IActionResult> GetUserInteractions(Guid userId, CancellationToken token)
            => Ok(await _sender.Send(new GetUserInteractionsQuery(userId), token));
    }
}
