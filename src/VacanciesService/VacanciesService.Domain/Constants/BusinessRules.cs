namespace VacanciesService.Domain.Constants
{
    public static class BusinessRules
    {
        public static class Vacancy
        {
            public const int TitleMaxLength = 150;
            public const int EmploymentTypeMaxLenght = 50;

            public const int CacheExpiresHours = 1;
            public const float MinPredictionScore = 0.6f;

            public static readonly TimeSpan DeletionAfterArchiveTime = TimeSpan.FromDays(30);
        }

        public static class Application
        {
            public const int StatusMaxLength = 50;
            public const string DefaultStatus = "Unread";
        }

        public static class Salary
        {
            public const string DefaultCurrency = "USD";
            public const string DefaultCurrencyType = "fiat";
        }

        public static class Token
        {
            public const double AccessTokenExpiresDays = 1;
            public const double RefreshTokenExpiresDays = 7;

            public const string AccessTokenName = "AccessToken";
            public const string RefreshTokenName = "RefreshToken";
        }

        public static class Roles
        {
            public const string Company = "Company";
            public const string User = "User";

            public static readonly IEnumerable<string> All = [Company, User];
        }

        public static class Recomendation
        {
            public const string TrainedModelFile = "vacancy_recomdations_model.zip";
        }
    }
}
