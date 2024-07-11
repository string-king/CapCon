using AutoMapper;
using Base.Contracts.DAL;

namespace Base.DAL.EF;

public class BaseDalDomainMapper<TLeftObject, TRightObject> : IDalMapper<TLeftObject, TRightObject>
    where TLeftObject : class where TRightObject : class
{
    private readonly IMapper _mapper;
    public BaseDalDomainMapper(IMapper mapper)
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