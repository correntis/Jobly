using MediatR;
using VacanciesService.Domain.Enums;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Vacancies.Queries.GetVacanciesByInteraction
{
    public sealed record GetVacanciesByInteractionQuery(
        Guid UserId,
        InteractionType InteractionType,
        int PageNumber,
        int PageSize) : IRequest<List<Vacancy>>;
}

