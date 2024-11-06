using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Exceptions;
using UsersService.Domain.Models;

namespace UsersService.Application.Users.Queries.GetUserQuery
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, User>
    {
        private readonly ILogger<GetUserQueryHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetUserQueryHandler(
            ILogger<GetUserQueryHandler> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        async Task<User> IRequestHandler<GetUserQuery, User>.Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start handling {QueryName} for user with ID {UserId}", request.GetType().Name, request.Id);

            var userEntity = await _unitOfWork.UsersRepository.GetAsync(request.Id, cancellationToken)
                ?? throw new EntityNotFoundException($"User with id {request.Id} not found");

            _logger.LogInformation("Successfully handled {QueryName} for user with ID {UserId}", request.GetType().Name, userEntity.Id);

            return _mapper.Map<User>(userEntity);
        }
    }
}
