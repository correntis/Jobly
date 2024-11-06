using UsersService.Domain.Enums;

namespace UsersService.Domain.Abstractions.Services
{
    public interface ITokensService
    {
        string CreateAccessToken(int id, IEnumerable<string> roles, DateTime expiresTime);
        string CreateRefreshToken();
        public TokenValidationResults ValidateToken(string token);
        public TokenValidationResults ValidateRoles(string token, IEnumerable<string> roles);
    }
}