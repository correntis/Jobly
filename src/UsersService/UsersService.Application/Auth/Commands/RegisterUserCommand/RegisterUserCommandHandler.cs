using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Abstractions.Services;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Exceptions;
using UsersService.Domain.Models;

namespace UsersService.Application.Auth.Commands.RegisterUserCommand
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Token>
    {
        private readonly ILogger<RegisterUserCommandHandler> _logger;
        private readonly IAuthorizationService _authService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RegisterUserCommandHandler(
            ILogger<RegisterUserCommandHandler> logger,
            IAuthorizationService authService,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _logger = logger;
            _authService = authService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Token> Handle(RegisterUserCommand request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Start handling {CommandName} for user with Email {Email}", request.GetType().Name, request.Email);

            var existingUserEntity = await _unitOfWork.UsersRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (existingUserEntity is not null)
            {
                throw new EntityAlreadyExistsException($"User with email {request.Email} already exists");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var userEntity = _mapper.Map<UserEntity>(request);

            userEntity.PasswordHash = passwordHash;
            userEntity.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.UsersRepository.AddAsync(userEntity, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var token = await _authService.IssueTokenAsync(userEntity.Id, [userEntity.Type], cancellationToken);

            _logger.LogInformation("Successfully handled {CommandName} for user with ID {UserId}", request.GetType().Name, userEntity.Id);

            return token;
        }
    }
}
