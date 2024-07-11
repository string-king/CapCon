using AutoMapper;

namespace App.BLL.DTO;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<App.Domain.Company, App.BLL.DTO.Company>().ReverseMap();
        CreateMap<App.Domain.Loan, App.BLL.DTO.Loan>().ReverseMap();
        CreateMap<App.Domain.LoanOffer, App.BLL.DTO.LoanOffer>().ReverseMap();
        CreateMap<App.Domain.LoanRequest, App.BLL.DTO.LoanRequest>().ReverseMap();
        CreateMap<App.Domain.Transaction, App.BLL.DTO.Transaction>().ReverseMap();
        CreateMap<App.Domain.UserCompany, App.BLL.DTO.UserCompany>().ReverseMap();
        CreateMap<App.Domain.Identity.AppUser, App.BLL.DTO.Identity.AppUser>().ReverseMap();

    }
}