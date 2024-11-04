using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Exceptions;

namespace UsersService.Application.Users.Commands.AddUserCommand
{
    public class AddUserCommandHandler : IRequestHandler<AddUserCommand, int>
    {
        private readonly ILogger<AddUserCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AddUserCommandHandler(
            ILogger<AddUserCommandHandler> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> Handle(AddUserCommand request, CancellationToken cancellationToken)
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

            _logger.LogInformation("Successfully handled {command}", request.GetType().Name);

            return userEntity.Id;
        }
    }
}
