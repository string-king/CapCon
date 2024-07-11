using AutoMapper;

namespace WebApp.Helpers;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<App.BLL.DTO.Company, App.DTO.v1_0.Company>().ReverseMap();
        CreateMap<App.BLL.DTO.Company, App.DTO.v1_0.CompanySimple>().ReverseMap();

        
        CreateMap<App.BLL.DTO.Loan, App.DTO.v1_0.Loan>().ReverseMap();
        
        CreateMap<App.BLL.DTO.LoanOffer, App.DTO.v1_0.LoanOffer>().ReverseMap();
        
        CreateMap<App.BLL.DTO.LoanRequest, App.DTO.v1_0.LoanRequest>().ReverseMap();
        CreateMap<App.BLL.DTO.LoanRequest, App.DTO.v1_0.LoanRequestSimple>().ReverseMap();

        
        CreateMap<App.BLL.DTO.Transaction, App.DTO.v1_0.Transaction>().ReverseMap();
        
        CreateMap<App.BLL.DTO.UserCompany, App.DTO.v1_0.UserCompany>().ReverseMap();
        CreateMap<App.BLL.DTO.UserCompany, App.DTO.v1_0.UserCompanyForCompany>().ReverseMap();
        
        CreateMap<App.BLL.DTO.Identity.AppUser, App.DTO.v1_0.Identity.AppUser>().ReverseMap();
    }
}