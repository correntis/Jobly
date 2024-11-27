namespace VacanciesService.Domain.Models
{
    public sealed record TokenValidationResult(
        bool IsValidToken,
        bool IsValidRoles,
        bool IsAccessTokenRefreshed,
        string NewAccessToken);
}