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
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public LoginUserCommandHandler(
            ILogger<LoginUserCommandHandler> logger,
            IAuthorizationService authService,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _authService = authService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<(User, Token)> Handle(LoginUserCommand request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Start handling {CommandName} for user with Email {Email}", request.GetType().Name, request.Email);

            var userEntity = await _unitOfWork.UsersRepository.GetByEmailAsync(request.Email)
                ?? throw new EntityNotFoundException($"User with email {request.Email} not found");

            var isValidPassword = await _unitOfWork.UsersRepository.CheckPasswordAsync(userEntity, request.Password);

            if (!isValidPassword)
            {
                throw new InvalidPasswordException($"Invalid password for user with email {request.Email}");
            }

            var user = _mapper.Map<User>(userEntity);

            var userRoles = await _unitOfWork.UsersRepository.GetRolesAsync(userEntity);

            var token = await _authService.IssueTokenAsync(userEntity.Id, userRoles, cancellationToken);

            _logger.LogInformation("Successfully handled {CommandName} for user ID {UserId}", request.GetType().Name, userEntity.Id);

            return (user, token);
        }
    }
}
