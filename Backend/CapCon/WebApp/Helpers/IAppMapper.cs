namespace WebApp.Helpers;

public interface IAppMapper<TLeftObject, TRightObject>
    where TLeftObject : class
    where TRightObject : class
{
    TLeftObject? Map(TRightObject? inObject);
    TRightObject? Map(TLeftObject? inObject);

}