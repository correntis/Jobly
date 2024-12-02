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
        [Route("users/{userId}&pageNumber={pageNumber}&pageSize={pageSize}")]
        public async Task<IActionResult> GetByUser(Guid userId, int pageNumber, int pageSize, CancellationToken token)
              => Ok(await _sender.Send(new GetApplicationsPageByUserQuery(userId, pageNumber, pageSize), token));

        [HttpGet]
        [Route("vacancies/{vacancyId}&pageNumber={pageNumber}&pageSize={pageSize}")]
        public async Task<IActionResult> GetByVacancy(Guid vacancyId, int pageNumber, int pageSize, CancellationToken token)
            => Ok(await _sender.Send(new GetApplicationsPageByVacancyQuery(vacancyId, pageNumber, pageSize), token));
    }
}
