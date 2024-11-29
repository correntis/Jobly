using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UsersService.Application.Auth.Commands.LoginUserCommand;
using UsersService.Application.Auth.Commands.RegisterUserCommand;
using UsersService.Domain.Constants;
using UsersService.Domain.Models;

namespace UsersService.Presentation.Controllers.Http
{
    [ApiController]
    [Route("/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserCommand registerUserCommand, CancellationToken cancellationToken)
        {
            var (userId, token) = await _mediator.Send(registerUserCommand, cancellationToken);

            AppendTokenToCookie(token);

            return Ok(userId);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserCommand loginUserCommand, CancellationToken cancellationToken)
        {
            var (user, token) = await _mediator.Send(loginUserCommand, cancellationToken);

            AppendTokenToCookie(token);

            return Ok(user);
        }

        private void AppendTokenToCookie(Token token)
        {
            AppendCookie(
                BusinessRules.Token.AccessTokenName,
                token.AccessToken,
                DateTime.UtcNow.AddDays(BusinessRules.Token.AccessTokenExpiresDays));

            AppendCookie(
                BusinessRules.Token.RefreshTokenName,
                token.RefreshToken,
                DateTime.UtcNow.AddDays(BusinessRules.Token.RefreshTokenExpiresDays));
        }

        private void AppendCookie(string key, string value, DateTime expiresTime)
        {
            var cookieOptions = new CookieOptions()
            {
                Expires = expiresTime,
            };

            HttpContext.Response.Cookies.Append(key, value, cookieOptions);
        }
    }
}
