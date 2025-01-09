using Jobly.Brokers.Abstractions;
using Jobly.Brokers.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Entities.NoSQL;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Exceptions;

namespace UsersService.Application.Companies.Commands.ViewResume
{
    public class ViewResumeCommandHandler : IRequestHandler<ViewResumeCommand>
    {
        private readonly ILogger<ViewResumeCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBrokerProcuder _brokerProcuder;

        public ViewResumeCommandHandler(
            ILogger<ViewResumeCommandHandler> logger,
            IUnitOfWork unitOfWork,
            IBrokerProcuder brokerProcuder)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _brokerProcuder = brokerProcuder;
        }

        public async Task Handle(ViewResumeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Start handling {CommandName} for company with ID {CompanyId} and resume with ID {ResumeId}",
                request.GetType().Name,
                request.CompanyId,
                request.ResumeId);

            var companyEntity = await _unitOfWork.CompaniesRepository.GetAsync(request.CompanyId, cancellationToken)
                ?? throw new EntityNotFoundException($"Company with id {request.CompanyId} not found");

            var resumeEntity = await _unitOfWork.ResumesRepository.GetAsync(request.ResumeId, cancellationToken)
                ?? throw new EntityNotFoundException($"Resume with id {request.ResumeId} not found");

            _ = await _unitOfWork.UsersRepository.GetByIdAsync(resumeEntity.UserId)
                ?? throw new EntityNotFoundException($"User with id {resumeEntity.Id} not found");

            await ProduceResumeViewEventAsync(companyEntity, resumeEntity);

            _logger.LogInformation(
                "Successfully handled {CommandName} for company with ID {CompanyId} and resume with ID {ResumeId}",
                request.GetType().Name,
                request.CompanyId,
                request.ResumeId);
        }

        public async Task ProduceResumeViewEventAsync(CompanyEntity companyEntity, ResumeEntity resumeEntity)
        {
            var viewEvent = new ResumeViewEvent()
            {
                UserId = resumeEntity.UserId,
                CompanyId = companyEntity.Id,
                CompanyName = companyEntity.Name,
            };

            await _brokerProcuder.SendAsync(viewEvent);
        }
    }
}
