using Microsoft.Extensions.Logging;
using VacanciesService.Application.Vacancies.Recommendations.ML;
using VacanciesService.Domain.Abstractions.Services;

namespace VacanciesService.Application.Vacancies.Recommendations.Jobs
{
    public class TrainVacancyRecommendationsJob
    {
        private readonly ILogger<TrainVacancyRecommendationsJob> _logger;
        private readonly IUsersService _usersService;
        private readonly VacancyRecommendationsModel _model;

        public TrainVacancyRecommendationsJob(
            ILogger<TrainVacancyRecommendationsJob> logger,
            IUsersService usersService,
            VacancyRecommendationsModel model)
        {
            _logger = logger;
            _usersService = usersService;
            _model = model;
        }

        public async Task ExecuteAsync()
        {
            _logger.LogInformation("[ML] Start training vacancies recommendations model");

            await Task.Factory.StartNew(
                () => _model.TrainModel(_model.LoadData()),
                TaskCreationOptions.LongRunning);

            _model.RefreshModel();

            _logger.LogInformation("[ML] Successfully trained vacancies recommendations model");
        }
    }
}
