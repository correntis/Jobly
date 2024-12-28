using AutoMapper;
using UsersService.Application.Companies.Commands.AddCompany;
using UsersService.Application.Companies.Commands.UpdateCompany;
using UsersService.Domain.Entities.SQL;
using UsersService.Domain.Models;

namespace UsersService.Application.Companies.Mapping
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            CreateMap<AddCompanyCommand, CompanyEntity>();
            CreateMap<UpdateCompanyCommand, CompanyEntity>();
            CreateMap<CompanyEntity, Company>();
        }
    }
}
