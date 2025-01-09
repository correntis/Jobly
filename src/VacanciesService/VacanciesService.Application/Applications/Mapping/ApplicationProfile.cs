using AutoMapper;
using VacanciesService.Application.Applications.Commands.AddApplication;
using VacanciesService.Application.Applications.Commands.UpdateApplication;
using VacanciesService.Domain.Entities.SQL;

namespace VacanciesService.Application.Applications.Mapping
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<AddApplicationCommand, ApplicationEntity>();
            CreateMap<UpdateApplicationCommand, ApplicationEntity>();
        }
    }
}
