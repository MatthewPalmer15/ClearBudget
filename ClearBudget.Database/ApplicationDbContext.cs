using ClearBudget.Database.Entities.Client;
using ClearBudget.Database.Entities.Settings;
using ClearBudget.Database.Entities.Transactions;
using ClearBudget.Database.Extensions;
using ClearBudget.Infrastructure.Encryption;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ClearBudget.Database;

public class ApplicationDbContext(IEncryptionService encryptionService, IConfiguration configuration) : DbContext, IDbContext
{
    public DbSet<ClientUser> ClientUsers { get; set; }
    public DbSet<ClientUserLogin> ClientUserLogins { get; set; }
    public DbSet<ClientUserClaim> ClientUserClaims { get; set; }
    public DbSet<ClientUserRole> ClientUserRoles { get; set; }
    public DbSet<ClientRole> ClientRoles { get; set; }
    public DbSet<ClientRoleClaim> ClientRoleClaims { get; set; }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<AccountTransaction> AccountTransactions { get; set; }
    public DbSet<AccountTransactionCategory> AccountTransactionCategories { get; set; }

    public DbSet<Setting> Settings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("dbo");
        modelBuilder.ApplyConfigurations();
        modelBuilder.UseEncryption(encryptionService.EncryptionProvider);

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("Default"));
        optionsBuilder.UseLazyLoadingProxies();

        base.OnConfiguring(optionsBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var rowsAffected = await base.SaveChangesAsync(cancellationToken);
            return rowsAffected;
        }
        catch
        {
            return -1;
        }
    }
}
