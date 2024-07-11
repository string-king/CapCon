using AutoMapper;

namespace App.BLL;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<App.DAL.DTO.Company, App.BLL.DTO.Company>().ReverseMap();
        CreateMap<App.DAL.DTO.Loan, App.BLL.DTO.Loan>().ReverseMap();
        CreateMap<App.DAL.DTO.LoanOffer, App.BLL.DTO.LoanOffer>().ReverseMap();
        CreateMap<App.DAL.DTO.LoanRequest, App.BLL.DTO.LoanRequest>().ReverseMap();
        CreateMap<App.DAL.DTO.Transaction, App.BLL.DTO.Transaction>().ReverseMap();
        CreateMap<App.DAL.DTO.UserCompany, App.BLL.DTO.UserCompany>().ReverseMap();
        
        CreateMap<App.DAL.DTO.Identity.AppUser, App.BLL.DTO.Identity.AppUser>().ReverseMap();
    }
}