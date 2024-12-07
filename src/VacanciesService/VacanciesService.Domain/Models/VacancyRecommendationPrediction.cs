using Microsoft.ML.Data;

namespace VacanciesService.Domain.Models
{
    public class VacancyRecommendationPrediction
    {
        [ColumnName("Score")]
        public float PredictedInteractionType { get; set; }
    }
}
