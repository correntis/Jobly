using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Abstractions.Services;
using UsersService.Domain.Exceptions;
using UsersService.Domain.Models;

namespace UsersService.Application.Auth.Commands.LoginUserCommand
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, (User, Token)>
    {
        private readonly ILogger<LoginUserCommandHandler> _logger;
        private readonly IAuthorizationService _authService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LoginUserCommandHandler(
            ILogger<LoginUserCommandHandler> logger,
            IAuthorizationService authService,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _logger = logger;
            _authService = authService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<(User, Token)> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start handling {command}", request.GetType().Name);

            var userEntity = await _unitOfWork.UsersRepository.GetByEmailAsync(request.Email, cancellationToken)
                ?? throw new EntityNotFoundException($"User with email {request.Email} not found");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, userEntity.PasswordHash))
            {
                throw new InvalidPasswordException("Invalid password");
            }

            var user = _mapper.Map<User>(userEntity);

            var token = await _authService.IssueTokenAsync(userEntity.Id, [userEntity.Type], cancellationToken);

            _logger.LogInformation("Successfully handled {command}", request.GetType().Name);

            return (user, token);
        }
    }
}
