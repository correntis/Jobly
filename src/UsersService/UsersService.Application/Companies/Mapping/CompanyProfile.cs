using AutoMapper;
using UsersService.Application.Companies.Commands.AddCompanyCommand;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Models;

namespace UsersService.Application.Companies.Mapping
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            CreateMap<AddCompanyCommand, CompanyEntity>();
            CreateMap<CompanyEntity, Company>();
        }
    }
}
