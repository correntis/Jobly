using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Exceptions;
using UsersService.Domain.Models;

namespace UsersService.Application.Users.Queries.GetUser
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, User>
    {
        private readonly ILogger<GetUserQueryHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public GetUserQueryHandler(
            ILogger<GetUserQueryHandler> logger,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<User> Handle(GetUserQuery request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Start handling {QueryName} for user with ID {UserId}", request.GetType().Name, request.Id);

            var userEntity = await _unitOfWork.UsersRepository.GetByIdAsync(request.Id)
                ?? throw new EntityNotFoundException($"User with id {request.Id} not found");

            _logger.LogInformation("Successfully handled {QueryName} for user with ID {UserId}", request.GetType().Name, userEntity.Id);

            return _mapper.Map<User>(userEntity);
        }
    }
}
