using Base.Contracts.DAL;

namespace Base.Contracts.BLL;

public interface IBLLMapper<TLeftObject, TRightObject> : IDalMapper<TLeftObject, TRightObject> 
    where TLeftObject : class 
    where TRightObject : class
{
    
}