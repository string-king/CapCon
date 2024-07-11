using AutoMapper;
using Base.Contracts.BLL;

namespace App.BLL.DTO;

public class BllDtoDomainMapper<TLeftObject, TRightObject> : IBLLMapper<TLeftObject, TRightObject>
    where TLeftObject : class where TRightObject : class
{
    private readonly IMapper _mapper;
    public BllDtoDomainMapper(IMapper mapper)
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