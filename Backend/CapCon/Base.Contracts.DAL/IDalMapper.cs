namespace Base.Contracts.DAL;

public interface IDalMapper<TLeftObject, TRightObject>
    where TLeftObject : class
    where TRightObject : class
{
    TLeftObject? Map(TRightObject? inObject);
    TRightObject? Map(TLeftObject? inObject);

}