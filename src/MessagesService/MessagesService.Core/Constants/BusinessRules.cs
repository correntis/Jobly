namespace MessagesService.Core.Constants
{
    public static class BusinessRules
    {
        public static class Roles
        {
            public const string Company = "Company";
            public const string User = "User";

            public static readonly IEnumerable<string> All = [Company, User];
        }

        public static class Token
        {
            public const double AccessTokenExpiresDays = 1;
            public const double RefreshTokenExpiresDays = 7;

            public const string AccessTokenName = "AccessToken";
            public const string RefreshTokenName = "RefreshToken";
        }
    }
}
