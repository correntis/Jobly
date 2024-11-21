using MediatR;
using Microsoft.AspNetCore.Mvc;
using VacanciesService.Application.VacanciesDetails.Commands.AddVacancyDetailsCommand;
using VacanciesService.Application.VacanciesDetails.Commands.DeleteVacancyDetailsCommand;
using VacanciesService.Application.VacanciesDetails.Queries.GetFilteredVacanciesDetailsQuery;
using VacanciesService.Domain.Abstractions.Services;
using VacanciesService.Domain.Filters.VacancyDetails;

namespace VacanciesService.Presentation.Controllers
{
    [ApiController]
    [Route("/vacancies")]
    public class VacanciesController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly ICurrencyApiService _currencyApi;

        public VacanciesController(
            ISender sender,
            ICurrencyApiService currencyApi)
        {
            _sender = sender;
            _currencyApi = currencyApi;
        }

        [HttpPost]
        [Route("details")]
        public async Task<IActionResult> AddDetails(AddVacancyDetailsCommand command, CancellationToken token)
            => Ok(await _sender.Send(command, token));

        [HttpDelete]
        [Route("details/{id}")]
        public async Task<IActionResult> DeleteDetails(string id, CancellationToken token)
            => Ok(await _sender.Send(new DeleteVacancyDetailsCommand(id), token));

        [HttpPost]
        [Route("details/search")]
        public async Task<IActionResult> SearchDetails(VacancyDetailsFilter filter, CancellationToken token)
            => Ok(await _sender.Send(new GetFilteredVacanciesDetailsQuery(filter), token));
    }
}
