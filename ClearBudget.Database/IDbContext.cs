using ClearBudget.Database.Entities.Client;
using ClearBudget.Database.Entities.Settings;
using ClearBudget.Database.Entities.Transactions;
using Microsoft.EntityFrameworkCore;

namespace ClearBudget.Database;

public interface IDbContext
{
    DbSet<ClientUser> ClientUsers { get; set; }
    DbSet<ClientUserLogin> ClientUserLogins { get; set; }
    DbSet<ClientUserClaim> ClientUserClaims { get; set; }
    DbSet<ClientUserRole> ClientUserRoles { get; set; }
    DbSet<ClientRole> ClientRoles { get; set; }
    DbSet<ClientRoleClaim> ClientRoleClaims { get; set; }
    DbSet<Account> Accounts { get; set; }
    DbSet<AccountTransaction> AccountTransactions { get; set; }
    DbSet<AccountTransactionCategory> AccountTransactionCategories { get; set; }
    DbSet<Setting> Settings { get; set; }

    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
