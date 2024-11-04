namespace UsersService.Domain.Constants
{
    public static class BusinessRules
    {
        public static class User
        {
            public const int MaxFirstNameLength = 50;
            public const int MaxLastNameLength = 50;
            public const int MaxTypeLength = 20;
            public const int MaxEmailLength = 30;
            public const int MaxPhoneLength = 20;
            public const int MinPasswordLength = 6;
            public static string[] Types { get; } = ["company", "user"];
        }

        public static class Company
        {
            public const int MaxNameLength = 50;
            public const int MaxDesctiptionLength = 1000;
            public const int MaxCityLength = 30;
            public const int MaxAddressLength = 100;
            public const int MaxEmailLength = 30;
            public const int MaxPhoneLength = 20;
            public const int MaxWebSiteLength = 50;
            public const int MaxTypeLength = 30;
        }
    }
}
