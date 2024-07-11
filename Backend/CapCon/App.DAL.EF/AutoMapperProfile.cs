using AutoMapper;

namespace App.DAL.EF;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<App.Domain.Company, App.DAL.DTO.Company>().ReverseMap();
        CreateMap<App.Domain.Loan, App.DAL.DTO.Loan>().ReverseMap();
        CreateMap<App.Domain.LoanOffer, App.DAL.DTO.LoanOffer>().ReverseMap();
        CreateMap<App.Domain.LoanRequest, App.DAL.DTO.LoanRequest>().ReverseMap();
        CreateMap<App.Domain.Transaction, App.DAL.DTO.Transaction>().ReverseMap();
        CreateMap<App.Domain.UserCompany, App.DAL.DTO.UserCompany>().ReverseMap();
        
        CreateMap<App.Domain.Identity.AppUser, App.DAL.DTO.Identity.AppUser>().ReverseMap();
    }
}