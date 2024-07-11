using Base.Contracts.BLL;
using Base.Contracts.DAL;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace Base.BLL;

public abstract class 
    BaseBLL<TAppDbContext> : IBLL
where TAppDbContext : DbContext
{
    protected readonly IUnitOfWork UoW;

    protected BaseBLL(IUnitOfWork uoW)
    {
        UoW = uoW;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await UoW.SaveChangesAsync();
    }
}