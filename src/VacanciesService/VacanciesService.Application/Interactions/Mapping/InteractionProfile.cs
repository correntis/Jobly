using AutoMapper;
using VacanciesService.Application.Interactions.Commands.AddInteraction;
using VacanciesService.Domain.Entities.SQL;
using VacanciesService.Domain.Enums;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Interactions.Mapping
{
    public class InteractionProfile : Profile
    {
        public InteractionProfile()
        {
            CreateMap<AddInteractionCommand, VacancyInteractionEntity>()
                .ForMember(interaction => interaction.Type, m => m.MapFrom(command => (int)command.Type));

            CreateMap<VacancyInteractionEntity, VacancyInteraction>()
                .ForMember(interact => interact.Type, m => m.MapFrom(interactEntity => (InteractionType)interactEntity.Type));
        }
    }
}
