using App.Domain;
using AutoMapper;
using Base.Contracts.DAL;

namespace App.DAL.EF;

public class DalDomainMapper<TLeftObject, TRightObject> : IDalMapper<TLeftObject, TRightObject>
    where TLeftObject : class where TRightObject : class
{
    private readonly IMapper _mapper;
    public DalDomainMapper(IMapper mapper)
    {
        _mapper = mapper;
    }
    
    public TLeftObject? Map(TRightObject? inObject)
    {
        return _mapper.Map<TLeftObject>(inObject);
    }

    public TRightObject? Map(TLeftObject? inObject)
    {
        return _mapper.Map<TRightObject>(inObject); 
     }
}