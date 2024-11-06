using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UsersService.Domain.Abstractions.Services;
using UsersService.Domain.Configuration;
using UsersService.Domain.Enums;

namespace UsersService.Application.Services
{
    public class TokensService : ITokensService
    {
        private readonly ILogger<TokensService> _logger;
        private readonly IOptions<JwtOptions> _jwtOptions;

        public TokensService(
            ILogger<TokensService> logger,
            IOptions<JwtOptions> jwtOptions)
        {
            _logger = logger;
            _jwtOptions = jwtOptions;
        }

        public string CreateAccessToken(int id, IEnumerable<string> roles, DateTime expiresTime)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.Secret));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var jwtClaims = new List<Claim>()
            {
                new Claim("Id", id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, id.ToString()),
            };

            foreach (var role in roles)
            {
                jwtClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Value.Issuer,
                audience: _jwtOptions.Value.Audience,
                claims: jwtClaims,
                expires: expiresTime,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string CreateRefreshToken()
        {
            var tokenLength = 256;
            var characters = new char[]
            {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
                'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
                'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '_', '-', '.',
                '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '+', '=',
            };

            var random = new Random();
            var token = new StringBuilder();

            for (var i = 0; i < tokenLength; i++)
            {
                token.Append(characters[random.Next(characters.Length)]);
            }

            return token.ToString();
        }

        public TokenValidationResults ValidateRoles(string token, IEnumerable<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            if (tokenHandler.ReadToken(token) is not JwtSecurityToken jwtToken)
            {
                return TokenValidationResults.Failure;
            }

            var claims = jwtToken.Claims;

            var userRoles = claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            if (roles.Any(role => userRoles.Contains(role)))
            {
                return TokenValidationResults.Success;
            }

            return TokenValidationResults.Failure;
        }

        public TokenValidationResults ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtOptions.Value.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtOptions.Value.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.Secret)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

                if (validatedToken is JwtSecurityToken jwtToken &&
                    jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return TokenValidationResults.Success;
                }
            }
            catch (SecurityTokenExpiredException)
            {
                return TokenValidationResults.SuccessExpired;
            }
            catch (Exception)
            {
                return TokenValidationResults.Failure;
            }

            return TokenValidationResults.Failure;
        }
    }
}
