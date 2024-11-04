using AutoMapper;
using UsersService.Application.Resumes.Commands.AddResumeCommand;
using UsersService.Application.Resumes.Commands.UpdateResumeCommand;
using UsersService.Domain.Entities.NoSQL;
using UsersService.Domain.Models;

namespace UsersService.Application.Resumes.Mapping
{
    public class ResumeProfile : Profile
    {
        public ResumeProfile()
        {
            CreateMap<ResumeEntity, Resume>();
            CreateMap<AddResumeCommand, ResumeEntity>();
            CreateMap<UpdateResumeCommand, ResumeEntity>();

            CreateMap<CertificationEntity, Certification>().ReverseMap();
            CreateMap<EducationEntity, Education>().ReverseMap();
            CreateMap<JobExpirienceEntity, JobExpirience>().ReverseMap();
            CreateMap<LanguageEntity, Language>().ReverseMap();
            CreateMap<ProjectEntity, Project>().ReverseMap();
        }
    }
}
