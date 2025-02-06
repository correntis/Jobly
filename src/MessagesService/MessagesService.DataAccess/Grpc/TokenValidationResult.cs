namespace MessagesService.Core.Models
{
    public sealed record TokenValidationResult(
        bool IsValidToken,
        bool IsValidRoles,
        bool IsAccessTokenRefreshed,
        string NewAccessToken);
}
