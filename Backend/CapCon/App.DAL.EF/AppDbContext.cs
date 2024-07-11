using App.Domain;
using App.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid, IdentityUserClaim<Guid>, 
    AppUserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    public DbSet<AppRole> AppRoles { get; set; } = default!;
    public DbSet<AppUser> AppUsers { get; set; } = default!;
    public DbSet<AppUserRole> AppUserRoles { get; set; } = default!;
    public DbSet<Company> Companies { get; set; } = default!;

    public DbSet<Loan> Loans { get; set; } = default!;
    public DbSet<LoanOffer> LoanOffers { get; set; } = default!;
    public DbSet<LoanRequest> LoanRequests { get; set; } = default!;
    public DbSet<Transaction> Transactions { get; set; } = default!;
    public DbSet<UserCompany> UserCompanies { get; set; } = default!;

    public DbSet<AppRefreshToken> RefreshTokens { get; set; } = default!;

    public AppDbContext(DbContextOptions options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        
        foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            // disable cascade delete
            // relationship.DeleteBehavior = DeleteBehavior.Restrict;
            
            // enable cascade delete
            relationship.DeleteBehavior = DeleteBehavior.Cascade;
        }
        
    }

    public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entity in ChangeTracker.Entries().Where(e => e.State != EntityState.Deleted))
        {
            foreach (var prop in entity
                         .Properties
                         .Where(x => x.Metadata.ClrType == typeof(DateTime)))
            {
                prop.CurrentValue = ((DateTime) prop.CurrentValue).ToUniversalTime();
            }
        }

        return await base.SaveChangesAsync(cancellationToken);

    }
}