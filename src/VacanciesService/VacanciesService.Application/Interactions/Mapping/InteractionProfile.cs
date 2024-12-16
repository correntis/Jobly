using AutoMapper;
using VacanciesService.Application.Interactions.Commands.AddInteractionCommand;
using VacanciesService.Domain.Entities.SQL;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Interactions.Mapping
{
    public class InteractionProfile : Profile
    {
        public InteractionProfile()
        {
            CreateMap<AddInteractionCommand, VacancyInteractionEntity>();

            CreateMap<VacancyInteractionEntity, VacancyInteraction>();
        }
    }
}
