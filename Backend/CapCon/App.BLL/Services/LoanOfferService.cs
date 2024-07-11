using App.Contracts.BLL.Services;
using App.Contracts.DAL;
using App.Contracts.DAL.Repositories;
using App.DAL.DTO;
using App.DAL.EF;
using App.DAL.EF.Repositories;
using AutoMapper;
using Base.BLL;
using Base.Contracts.BLL;
using LoanOffer = App.BLL.DTO.LoanOffer;

namespace App.BLL.Services;

public class LoanOfferService : 
    BaseEntityService<App.DAL.DTO.LoanOffer, App.BLL.DTO.LoanOffer, ILoanOfferRepository>,
    ILoanOfferService
{
    private readonly IAppUnitOfWork _uow;
    private readonly IMapper _mapper;
    public LoanOfferService(IAppUnitOfWork uoW, ILoanOfferRepository repository, IMapper mapper) : base(uoW, repository, new BllDalMapper<App.DAL.DTO.LoanOffer, App.BLL.DTO.LoanOffer>(mapper))
    {
        _mapper = mapper;
        _uow = uoW;
    }

    public async Task<IEnumerable<LoanOffer>> GetAllLoanOffersIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        return (await _uow.LoanOffers.GetAllLoanOffersIncludingAsync(userId)).Select(e => Mapper.Map(e));
    }

    public async Task<LoanOffer?> FirstOrDefaultLoanOfferIncludingAsync(Guid id, Guid userId = default, bool noTracking = true)
    {
        return _mapper.Map<LoanOffer>(await _uow.LoanOffers.FirstOrDefaultLoanOfferIncludingAsync(id, userId, noTracking));
    }
}