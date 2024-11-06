using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Exceptions;
using UsersService.Domain.Models;

namespace UsersService.Application.Resumes.Queries.GetResumeByUser
{
    public class GetResumeByUserQueryHandler : IRequestHandler<GetResumeByUserQuery, Resume>
    {
        private readonly ILogger<GetResumeByUserQuery> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetResumeByUserQueryHandler(
            ILogger<GetResumeByUserQuery> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Resume> Handle(GetResumeByUserQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start handling {CommandName} for user with ID {UserId}", request.GetType().Name, request.UserId);

            var resumeEntity = await _unitOfWork.ResumesRepository.GetByAsync(r => r.UserId, request.UserId, cancellationToken)
                ?? throw new EntityNotFoundException($"Resume for user with id {request.UserId} not found");

            _logger.LogInformation("Successfully handled {CommandName} for user with ID {UserId}", request.GetType().Name, request.UserId);

            return _mapper.Map<Resume>(resumeEntity);
        }
    }
}
