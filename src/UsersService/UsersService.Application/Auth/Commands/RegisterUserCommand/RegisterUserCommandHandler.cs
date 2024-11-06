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

        public async Task<Token> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start handling {command}", request.GetType().Name);

            if (await _unitOfWork.UsersRepository.GetByEmailAsync(request.Email, cancellationToken) is not null)
            {
                throw new EntityAlreadyExistsException($"User with email {request.Email} already exists");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var userEntity = _mapper.Map<UserEntity>(request);

            userEntity.PasswordHash = passwordHash;
            userEntity.CreatedAt = DateTime.Now;

            await _unitOfWork.UsersRepository.AddAsync(userEntity, cancellationToken);

            await _unitOfWork.SaveChangesAsync();

            var token = await _authService.IssueTokenAsync(userEntity.Id, [userEntity.Type], cancellationToken);

            _logger.LogInformation("Successfully handled {command}", request.GetType().Name);

            return token;
        }
    }
}
