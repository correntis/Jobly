using AutoMapper;
using Hangfire;
using Jobly.Brokers.Abstractions;
using Jobly.Brokers.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using UsersService.Application.Auth.Jobs;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Abstractions.Services;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Exceptions;
using UsersService.Domain.Models;

namespace UsersService.Application.Auth.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, (Guid, Token)>
    {
        private readonly ILogger<RegisterUserCommandHandler> _logger;
        private readonly IAuthorizationService _authService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBrokerProcuder _brokerProcuder;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly UserManager<UserEntity> _userManager;

        public RegisterUserCommandHandler(
            ILogger<RegisterUserCommandHandler> logger,
            IAuthorizationService authService,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IBrokerProcuder brokerProcuder,
            IBackgroundJobClient backgroundJobClient,
            UserManager<UserEntity> userManager)
        {
            _logger = logger;
            _authService = authService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _brokerProcuder = brokerProcuder;
            _backgroundJobClient = backgroundJobClient;
            _userManager = userManager;
        }

        public async Task<(Guid, Token)> Handle(RegisterUserCommand request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Start handling {CommandName} for user with Email {Email}", request.GetType().Name, request.Email);

            var userEntity = await CreateUserAsync(request);

            await CreateUserRolesAsync(userEntity, request.RolesNames);

            CreateNotifyRegistrationJob(userEntity);

            var token = await _authService.IssueTokenAsync(userEntity.Id, request.RolesNames, cancellationToken);

            _logger.LogInformation("Successfully handled {CommandName} for user with ID {UserId}", request.GetType().Name, userEntity.Id);

            return (userEntity.Id, token);
        }

        private async Task<UserEntity> CreateUserAsync(RegisterUserCommand request)
        {
            var existingUserEntity = await _unitOfWork.UsersRepository.GetByEmailAsync(request.Email);

            // Если пользователь существует и имеет полную регистрацию, выбрасываем исключение
            if(existingUserEntity is not null && existingUserEntity.IsFullRegistration)
            {
                throw new EntityAlreadyExistsException($"User with email {request.Email} already exists");
            }

            // Если пользователь существует, но регистрация неполная, удаляем его
            if(existingUserEntity is not null && !existingUserEntity.IsFullRegistration)
            {
                _logger.LogInformation(
                    "Found incomplete registration for email {Email}, deleting user {UserId}",
                    request.Email,
                    existingUserEntity.Id);

                var deleteResult = await _unitOfWork.UsersRepository.DeleteAsync(existingUserEntity);
                if(!deleteResult.Succeeded)
                {
                    _logger.LogWarning(
                        "Failed to delete incomplete user {UserId}: {Errors}",
                        existingUserEntity.Id,
                        string.Join(", ", deleteResult.Errors.Select(e => e.Description)));
                }
            }

            var userEntity = _mapper.Map<UserEntity>(request);
            userEntity.IsFullRegistration = request.IsFullRegistration;
            userEntity.CreatedAt = DateTime.UtcNow;

            var identityResult = await _unitOfWork.UsersRepository.AddAsync(userEntity, request.Password);

            if(!identityResult.Succeeded)
            {
                throw new ValidationException(identityResult.Errors.Select(e => new ValidationError(e.Code, e.Description)));
            }

            return userEntity;
        }

        private async Task CreateUserRolesAsync(UserEntity userEntity, IEnumerable<string> rolesNames)
        {
            if(rolesNames is null && !rolesNames.Any())
            {
                return;
            }

            foreach(var role in rolesNames)
            {
                if(!await _unitOfWork.RolesRepository.RoleExistsAsync(role))
                {
                    throw new EntityNotFoundException($"Role with name {role} not found");
                }
            }

            await _unitOfWork.UsersRepository.AddToRolesAsync(userEntity, rolesNames);
        }

        private void CreateNotifyRegistrationJob(UserEntity userEntity)
        {
            var registrationEvent = _mapper.Map<RegistrationEvent>(userEntity);

            _backgroundJobClient.Schedule<NotifyRegistrationJob>(
                j => j.ExecuteAsync(registrationEvent),
                TimeSpan.FromMinutes(5));
        }
    }
}
