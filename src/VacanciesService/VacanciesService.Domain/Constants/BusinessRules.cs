namespace VacanciesService.Domain.Constants
{
    public static class BusinessRules
    {
        public static class Vacancy
        {
            public const int TitleMaxLength = 150;
            public const int EmployementTypeMaxLenght = 50;
        }

        public static class Application
        {
            public const int StatusMaxLength = 50;
        }

        public static class Salary
        {
            public const string DefaultCurrency = "USD";
            public const string DefaultCurrencyType = "fiat";
        }
    }
}
