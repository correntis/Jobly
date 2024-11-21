using MediatR;
using Microsoft.AspNetCore.Mvc;
using VacanciesService.Application.Vacancies.Commands.AddVacancyCommand;
using VacanciesService.Application.Vacancies.Commands.ArchiveVacancyCommand;
using VacanciesService.Application.Vacancies.Commands.DeleteVacancyCommand;
using VacanciesService.Application.Vacancies.Queries.GetVacancyByCompanyQuery;
using VacanciesService.Application.Vacancies.Queries.GetVacancyQuery;
using VacanciesService.Application.VacanciesDetails.Commands.AddVacancyDetailsCommand;
using VacanciesService.Application.VacanciesDetails.Commands.DeleteVacancyDetailsCommand;
using VacanciesService.Application.VacanciesDetails.Queries.GetFilteredVacanciesDetailsQuery;
using VacanciesService.Application.VacanciesDetails.Queries.GetVacancyDetailsQuery;
using VacanciesService.Domain.Filters.VacancyDetails;

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
        public async Task<IActionResult> Add(AddVacancyCommand command, CancellationToken token)
            => Ok(await _sender.Send(command, token));

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken token)
            => Ok(await _sender.Send(new DeleteVacancyCommand(id), token));

        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> Archive(int id, CancellationToken token)
            => Ok(await _sender.Send(new ArchiveVacancyCommand(id), token));

        [HttpGet]
        [Route("{id}&{applications}")]
        public async Task<IActionResult> Get(int id, bool applications, CancellationToken token)
            => Ok(await _sender.Send(new GetVacancyQuery(id, applications), token));

        [HttpGet]
        [Route("companies/{companyId}")]
        public async Task<IActionResult> GetByCompany(int companyId, CancellationToken token)
            => Ok(await _sender.Send(new GetVacanciesByCompanyQuery(companyId), token));

        [HttpPost]
        [Route("details")]
        public async Task<IActionResult> AddDetails(AddVacancyDetailsCommand command, CancellationToken token)
            => Ok(await _sender.Send(command, token));

        [HttpDelete]
        [Route("details/{id}")]
        public async Task<IActionResult> DeleteDetails(string id, CancellationToken token)
            => Ok(await _sender.Send(new DeleteVacancyDetailsCommand(id), token));

        [HttpGet]
        [Route("details")]
        public async Task<IActionResult> GetDetailsByVacancy(int vacancyId, CancellationToken token)
            => Ok(await _sender.Send(new GetVacancyDetailsQuery(vacancyId), token));

        [HttpPost]
        [Route("details/search")]
        public async Task<IActionResult> SearchDetails(VacancyDetailsFilter filter, CancellationToken token)
            => Ok(await _sender.Send(new GetFilteredVacanciesDetailsQuery(filter), token));
    }
}
