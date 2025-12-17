using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Services;

namespace UsersService.Infrastructure.Services
{
    public class UnpValidationService : IUnpValidationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<UnpValidationService> _logger;
        private const string EgrApiBaseUrl = "http://grp.nalog.gov.by/api/grp-public/data";

        public UnpValidationService(
            IHttpClientFactory httpClientFactory,
            ILogger<UnpValidationService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<bool> ValidateUnpAsync(string unp, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(unp))
            {
                return false;
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var url = $"{EgrApiBaseUrl}?unp={unp}&charset=UTF-8&type=json";

                _logger.LogInformation("Validating UNP {Unp} via EGR API", unp);

                var response = await httpClient.GetAsync(url, cancellationToken);

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    _logger.LogWarning("UNP {Unp} not found in EGR", unp);
                    return false;
                }

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to validate UNP {Unp}. Status code: {StatusCode}", unp, response.StatusCode);
                    return false;
                }

                var result = await response.Content.ReadFromJsonAsync<EgrApiResponse>(cancellationToken: cancellationToken);

                if (result?.Row == null)
                {
                    _logger.LogWarning("UNP {Unp} validation returned null result", unp);
                    return false;
                }

                // Проверяем, что компания действующая
                var isActive = result.Row.Ckodsost == "1" && result.Row.Vkods == "Действующий";
                
                _logger.LogInformation("UNP {Unp} validation result: {IsValid}, Status: {Status}", 
                    unp, isActive, result.Row.Vkods);

                return isActive;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating UNP {Unp}", unp);
                return false;
            }
        }

        private class EgrApiResponse
        {
            public EgrApiRow? Row { get; set; }
        }

        private class EgrApiRow
        {
            public string? Vunp { get; set; }
            public string? Vnaimp { get; set; }
            public string? Vnaimk { get; set; }
            public string? Vpadres { get; set; }
            public string? Dreg { get; set; }
            public string? Nmns { get; set; }
            public string? Vmns { get; set; }
            public string? Ckodsost { get; set; }
            public string? Vkods { get; set; }
            public string? Dlikv { get; set; }
            public string? Vlikv { get; set; }
        }
    }
}

