using ClearBudget.Database;
using ClearBudget.Database.Entities.Transactions;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ClearBudget.Application.Services.Background;

public class RecurringTransactionsService(IServiceProvider serviceProvider) : IHostedService
{
    private readonly int _batchSize = 1000;
    private CancellationTokenSource? _cts;
    private bool _retry;

    private readonly IDbContext _context = serviceProvider.GetService<IDbContext>() ?? throw new ArgumentNullException(nameof(IDbContext));
    private readonly IMapper _mapper = serviceProvider.GetService<IMapper>() ?? throw new ArgumentNullException(nameof(IMapper));

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        while (!_cts.IsCancellationRequested)
        {
            try
            {
                var transactionsToAdd = new List<AccountTransaction>();

                var transactions = await GetTransactionsAsync(cancellationToken);
                foreach (var transaction in transactions)
                {
                    var transactionToAdd = _mapper.Map<AccountTransaction>(transaction);
                    transactionToAdd.Id = Guid.Empty;
                    transactionToAdd.TransactionDate = DateTime.UtcNow.Date;

                    transactionsToAdd.Add(transactionToAdd);
                }

                _context.AccountTransactions.AddRange(transactionsToAdd);
                await _context.SaveChangesAsync(cancellationToken);
                _retry = false;
            }
            catch
            {
                _retry = true;
            }

            try { await Task.Delay(_retry ? TimeSpan.FromMinutes(5) : TimeSpan.FromDays(1), cancellationToken); } catch { /* cancelled */ }
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (_cts is null) return;
        await _cts.CancelAsync();
        _cts.Dispose();
    }

    private async Task<List<AccountTransaction>> GetTransactionsAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;

        var query = (from at in _context.AccountTransactions
                     join a in _context.Accounts on at.AccountId equals a.Id
                     where !a.Deleted
                           && !a.DateClosed.HasValue
                           && !at.Deleted
                           && (
                               at.Recurring == AccountTransaction.TransactionRecurringTypeEnum.Daily ||
                               (at.Recurring == AccountTransaction.TransactionRecurringTypeEnum.Weekly && today == at.TransactionDate.Date.AddDays(7)) ||
                               (at.Recurring == AccountTransaction.TransactionRecurringTypeEnum.Monthly && today == at.TransactionDate.Date.AddMonths(1)) ||
                               (at.Recurring == AccountTransaction.TransactionRecurringTypeEnum.Yearly && today == at.TransactionDate.Date.AddYears(1)) ||
                               (at.Recurring == AccountTransaction.TransactionRecurringTypeEnum.Custom && at.RecurringCustomDays.HasValue && today == at.TransactionDate.Date.AddDays(at.RecurringCustomDays.Value))
                           )
                     orderby at.Id
                     select at).AsNoTracking();

        var transactions = new List<AccountTransaction>();
        var count = await query.CountAsync(cancellationToken);

        for (var skip = 0; skip < count; skip += _batchSize)
        {
            var batch = await query
                .Skip(skip)
                .Take(_batchSize)
                .ToListAsync(cancellationToken);

            if (batch.Count == 0) break;

            transactions.AddRange(batch);
            skip += batch.Count;
        }

        return transactions;
    }
}
