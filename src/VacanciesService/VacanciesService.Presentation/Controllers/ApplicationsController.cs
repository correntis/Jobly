using MediatR;
using Microsoft.AspNetCore.Mvc;
using VacanciesService.Application.Applications.Commands.AddApplicationCommand;
using VacanciesService.Application.Applications.Commands.UpdateApplicationCommand;
using VacanciesService.Application.Applications.Queries.GetApplicationsByUserQuery;
using VacanciesService.Application.Applications.Queries.GetApplicationsByVacancyQuery;

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
        public async Task<IActionResult> Add(AddApplicationCommand command, CancellationToken token)
            => Ok(await _sender.Send(command, token));

        [HttpPut]
        public async Task<IActionResult> Update(UpdateApplicationCommand command, CancellationToken token)
           => Ok(await _sender.Send(command, token));

        [HttpGet]
        [Route("users/{userId}")]
        public async Task<IActionResult> GetByUser(Guid userId, CancellationToken token)
              => Ok(await _sender.Send(new GetApplicationsByUserQuery(userId), token));

        [HttpGet]
        [Route("vacancies/{vacancyId}")]
        public async Task<IActionResult> GetByVacancy(Guid vacancyId, CancellationToken token)
            => Ok(await _sender.Send(new GetApplicationsByVacancyQuery(vacancyId), token));
    }
}
