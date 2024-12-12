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
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, (Guid, Token)>
    {
        private readonly ILogger<RegisterUserCommandHandler> _logger;
        private readonly IAuthorizationService _authService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserCommandHandler(
            ILogger<RegisterUserCommandHandler> logger,
            IAuthorizationService authService,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _authService = authService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<(Guid, Token)> Handle(RegisterUserCommand request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Start handling {CommandName} for user with Email {Email}", request.GetType().Name, request.Email);

            var userEntity = await CreateUserAsync(request);

            await CreateUserRolesAsync(userEntity, request.RolesNames);

            var token = await _authService.IssueTokenAsync(userEntity.Id, request.RolesNames, cancellationToken);

            _logger.LogInformation("Successfully handled {CommandName} for user with ID {UserId}", request.GetType().Name, userEntity.Id);

            return (userEntity.Id, token);
        }

        private async Task<UserEntity> CreateUserAsync(RegisterUserCommand request)
        {
            var existingUserEntity = await _unitOfWork.UsersRepository.GetByEmailAsync(request.Email);

            if (existingUserEntity is not null)
            {
                throw new EntityAlreadyExistsException($"User with email {request.Email} already exists");
            }

            var userEntity = _mapper.Map<UserEntity>(request);

            userEntity.CreatedAt = DateTime.UtcNow;

            var identityResult = await _unitOfWork.UsersRepository.AddAsync(userEntity, request.Password);

            if (!identityResult.Succeeded)
            {
                throw new ValidationException(identityResult.Errors.Select(e => new ValidationError(e.Code, e.Description)));
            }

            return userEntity;
        }

        private async Task CreateUserRolesAsync(UserEntity userEntity, IEnumerable<string> rolesNames)
        {
            if (rolesNames is null && !rolesNames.Any())
            {
                return;
            }

            foreach (var role in rolesNames)
            {
                if (!await _unitOfWork.RolesRepository.RoleExistsAsync(role))
                {
                    throw new EntityNotFoundException($"Role with name {role} not found");
                }
            }

            await _unitOfWork.UsersRepository.AddToRolesAsync(userEntity, rolesNames);
        }
    }
}
