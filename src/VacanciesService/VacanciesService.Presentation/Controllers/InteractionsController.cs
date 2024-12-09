using MediatR;
using Microsoft.AspNetCore.Mvc;
using VacanciesService.Application.Interactions.Commands.AddInteractionCommand;
using VacanciesService.Application.Interactions.Queries.GetUserInteractionsQuery;
using VacanciesService.Application.Interactions.Queries.GetVacancyInteractionsQuery;

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
        public async Task<IActionResult> AddInteraction(AddInteractionCommand command, CancellationToken token)
            => Ok(await _sender.Send(command, token));

        [HttpGet]
        [Route("vacancies/{vacancyId}")]
        public async Task<IActionResult> GetVacancyInteractions(Guid vacancyId, CancellationToken token)
            => Ok(await _sender.Send(new GetVacancyInteractionsQuery(vacancyId), token));

        [HttpGet]
        [Route("users/{userId}")]
        public async Task<IActionResult> GetUserInteractions(Guid userId, CancellationToken token)
            => Ok(await _sender.Send(new GetUserInteractionsQuery(userId), token));
    }
}
