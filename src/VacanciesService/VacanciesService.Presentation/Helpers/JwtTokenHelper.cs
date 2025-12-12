using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using VacanciesService.Domain.Constants;

namespace VacanciesService.Presentation.Helpers
{
    public static class JwtTokenHelper
    {
        public static Guid? GetUserIdFromToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                
                if (tokenHandler.ReadToken(token) is not JwtSecurityToken jwtToken)
                {
                    return null;
                }

                var idClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "Id");
                
                if (idClaim != null && Guid.TryParse(idClaim.Value, out Guid userId))
                {
                    return userId;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public static Guid? GetUserIdFromHttpContext(HttpContext context)
        {
            if (!context.Request.Cookies.TryGetValue(BusinessRules.Token.AccessTokenName, out string accessToken))
            {
                return null;
            }

            return GetUserIdFromToken(accessToken);
        }
    }
}

