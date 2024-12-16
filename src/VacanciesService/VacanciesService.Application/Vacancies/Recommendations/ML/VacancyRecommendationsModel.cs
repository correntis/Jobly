using Bogus;
using Microsoft.ML;
using VacanciesService.Domain.Constants;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Vacancies.Recommendations.ML
{
    public class VacancyRecommendationsModel
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;

        public VacancyRecommendationsModel()
        {
            _mlContext = new MLContext();

            _model = LoadModel();
        }

        public IDataView LoadData(List<TrainingVacancyRecommendationData> trainingData = null)
        {
            if (trainingData is null)
            {
                trainingData = GenerateTrainingData(1000);
            }

            return _mlContext.Data.LoadFromEnumerable(trainingData);
        }

        public ITransformer TrainModel(IDataView trainingData)
        {
            var pipeline = BuildPipeline();
            var model = pipeline.Fit(trainingData);

            SaveModel(model);

            return model;
        }

        public float PredictInteraction(TrainingVacancyRecommendationData newData)
        {
            var predictEngine =
                _mlContext.Model.CreatePredictionEngine<TrainingVacancyRecommendationData, VacancyRecommendationPrediction>(_model);

            var prediction = predictEngine.Predict(newData);

            return prediction.PredictedInteractionType;
        }

        public void SaveModel(ITransformer model)
        {
            using var fileStream = new FileStream(
                BusinessRules.Recomendation.TrainedModelFile,
                FileMode.Create,
                FileAccess.Write,
                FileShare.Write);

            _mlContext.Model.Save(model, null, fileStream);
        }

        public ITransformer LoadModel()
        {
            using var fileStream = new FileStream(
                BusinessRules.Recomendation.TrainedModelFile,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);

            return _mlContext.Model.Load(fileStream, out _);
        }

        public void RefreshModel()
        {
            _model = LoadModel();
        }

        public bool IsModelTrained()
        {
            return File.Exists(BusinessRules.Recomendation.TrainedModelFile);
        }

        private IEstimator<ITransformer> BuildPipeline()
        {
            var featuresName = "Features";

            return _mlContext.Transforms.Text.FeaturizeText(nameof(TrainingVacancyRecommendationData.ResumeSkills))
                .Append(_mlContext.Transforms.Text.FeaturizeText(nameof(TrainingVacancyRecommendationData.ResumeTags)))
                .Append(_mlContext.Transforms.Text.FeaturizeText(nameof(TrainingVacancyRecommendationData.VacancySkills)))
                .Append(_mlContext.Transforms.Text.FeaturizeText(nameof(TrainingVacancyRecommendationData.VacancyTags)))
                .Append(_mlContext.Transforms.Text.FeaturizeText(nameof(TrainingVacancyRecommendationData.VacancyLanguages)))
                .Append(_mlContext.Transforms.Text.FeaturizeText(nameof(TrainingVacancyRecommendationData.ResumeLanguages)))
                .Append(_mlContext.Transforms.Text.FeaturizeText(nameof(TrainingVacancyRecommendationData.VacancyExperience)))
                .Append(_mlContext.Transforms.Text.FeaturizeText(nameof(TrainingVacancyRecommendationData.VacancySalary)))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(nameof(TrainingVacancyRecommendationData.InteractionType)))

                .Append(_mlContext.Transforms.Concatenate(
                    featuresName,
                    nameof(TrainingVacancyRecommendationData.ResumeSkills),
                    nameof(TrainingVacancyRecommendationData.ResumeTags),
                    nameof(TrainingVacancyRecommendationData.ResumeLanguages),
                    nameof(TrainingVacancyRecommendationData.VacancySkills),
                    nameof(TrainingVacancyRecommendationData.VacancyTags),
                    nameof(TrainingVacancyRecommendationData.VacancyLanguages),
                    nameof(TrainingVacancyRecommendationData.VacancyExperience),
                    nameof(TrainingVacancyRecommendationData.VacancySalary),
                    nameof(TrainingVacancyRecommendationData.InteractionType)))

                .Append(_mlContext.Regression.Trainers.Sdca(
                    labelColumnName: nameof(TrainingVacancyRecommendationData.SuitabilityScore),
                    featureColumnName: featuresName,
                    maximumNumberOfIterations: 100));
        }

        private static readonly string[] _languages = ["English", "Russian", "German", "Spanish", "French"];
        private static readonly string[] _languageLevels = ["Beginner", "Intermediate", "Advanced", "Native"];

        private List<TrainingVacancyRecommendationData> GenerateTrainingData(int count)
        {
            var faker = new Faker();
            var languageFaker = new Faker<Language>()
                .RuleFor(l => l.Name, f => f.PickRandom(_languages))
                .RuleFor(l => l.Level, f => f.PickRandom(_languageLevels));

            var data = new List<TrainingVacancyRecommendationData>();

            for (int i = 0; i < count; i++)
            {
                var resumeSkills = string.Join(",", faker.Lorem.Random.WordsArray(5, 10).ToList());
                var resumeTags = string.Join(",", faker.Lorem.Random.WordsArray(3, 7).ToList());
                var resumeLanguages = string.Join(",", languageFaker.Generate(faker.Random.Int(0, 3)).Select(l => $"{l.Name}-{l.Level}"));

                var vacancySkills = string.Join(",", faker.Lorem.Random.WordsArray(5, 10).ToList());
                var vacancyTags = string.Join(",", faker.Lorem.Random.WordsArray(3, 7).ToList());
                var vacancyLanguages = string.Join(",", languageFaker.Generate(faker.Random.Int(0, 3)).Select(l => $"{l.Name}-{l.Level}"));
                var vacancyExperience = $"{faker.Random.Int(0, 3)}-{faker.Random.Int(3, 7)}";
                var vacancySalary = $"{BusinessRules.Salary.DefaultCurrency}:{faker.Random.Int(1000, 10000)}-{faker.Random.Int(10000, 100000)}";

                var interactionType = faker.Random.Int(0, 3);
                var suitabilityScore = faker.Random.Float(0f, 1f);

                var dataItem = new TrainingVacancyRecommendationData
                {
                    ResumeSkills = resumeSkills,
                    ResumeTags = resumeTags,
                    ResumeLanguages = resumeLanguages,

                    VacancySkills = vacancySkills,
                    VacancyTags = vacancyTags,
                    VacancyLanguages = vacancyLanguages,
                    VacancyExperience = vacancyExperience,
                    VacancySalary = vacancySalary,

                    InteractionType = interactionType,
                    SuitabilityScore = suitabilityScore,
                };

                data.Add(dataItem);
            }

            return data;
        }
    }
}
