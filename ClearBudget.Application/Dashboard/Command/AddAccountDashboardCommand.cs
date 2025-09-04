using ClearBudget.Database;
using ClearBudget.Database.Entities.Transactions;
using MediatR;

namespace ClearBudget.Application.Dashboard.Commands;

public class CreateAccountCommand : IRequest
{
    public Guid ClientUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal GrossAmount { get; set; }
    public decimal InterestRate { get; set; }
    public decimal NetAmount { get; set; }

    internal class Handler(IDbContext context) : IRequestHandler<CreateAccountCommand>
    {
        public async Task Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var account = new Account
            {
                Id = Guid.NewGuid(),
                //ClientUserId = request.ClientUserId,
                Title = request.Title,
                GrossAmount = request.GrossAmount,
                InterestRate = request.InterestRate,
                //NetAmount = request.NetAmount,
                Deleted = false,
                DateClosed = null,
                DateCreated = DateTime.UtcNow
            };

            context.Accounts.Add(account);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
