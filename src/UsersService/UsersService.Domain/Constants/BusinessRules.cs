namespace UsersService.Domain.Constants
{
    public static class BusinessRules
    {
        public static class Roles
        {
            public const string Company = "Company";
            public const string User = "User";

            public static readonly IEnumerable<string> All = [Company, User];
        }

        public static class User
        {
            public const int MaxFirstNameLength = 50;
            public const int MaxLastNameLength = 50;
            public const int MaxTypeLength = 20;
            public const int MaxEmailLength = 30;
            public const int MaxPhoneLength = 20;
            public const int MinPasswordLength = 6;
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
            public const int MaxUnpLength = 9;
        }

        public static class Token
        {
            public const double AccessTokenExpiresDays = 7;
            public const double RefreshTokenExpiresDays = 30;

            public const string AccessTokenName = "AccessToken";
            public const string RefreshTokenName = "RefreshToken";
        }

        public static class Image
        {
            public const string Folder = "Uploads/Images";

            public static readonly IEnumerable<string> AllowedExtensions = [".jpg", ".jpeg", ".png"];
        }
    }
}
