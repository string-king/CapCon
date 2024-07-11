using Base.Contracts.DAL;
using Microsoft.EntityFrameworkCore;

namespace Base.DAL.EF;

public abstract class BaseUnitOfWork<TAppDbContext> : IUnitOfWork
    where TAppDbContext : DbContext
{
     protected readonly TAppDbContext UowDbContext;

    protected BaseUnitOfWork(TAppDbContext dbContext)
    {
        UowDbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await UowDbContext.SaveChangesAsync();
    }
}