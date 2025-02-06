using Microsoft.AspNetCore.Mvc;
using VacanciesService.Domain.Abstractions.Services;
using VacanciesService.Domain.Constants;
using VacanciesService.Domain.Models;
using VacanciesService.Presentation.Middleware.Authorization;

namespace VacanciesService.Presentation.Controllers
{
    [ApiController]
    [Route("/currencies")]
    public class CurrenciesController : ControllerBase
    {
        private readonly ICurrencyApiService _currencyApi;

        public CurrenciesController(ICurrencyApiService currencyApi)
        {
            _currencyApi = currencyApi;
        }

        [HttpGet]
        [AuthorizeRole(Roles = BusinessRules.Roles.User)]
        [AuthorizeRole(Roles = BusinessRules.Roles.Company)]
        public async Task<ActionResult<List<Currency>>> GetCurrencies(CancellationToken token)
        {
            return Ok(await _currencyApi.GetCurrenciesAsync(token));
        }
    }
}
