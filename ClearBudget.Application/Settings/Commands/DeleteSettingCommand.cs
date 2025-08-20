using ClearBudget.Database;
using ClearBudget.Database.Entities.Settings;
using ClearBudget.Infrastructure.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClearBudget.Application.Settings.Commands;

public class DeleteSettingCommand : IRequest<BaseResponse>
{
    public Guid Id { get; set; }

    internal class Handler(IDbContext context) : IRequestHandler<DeleteSettingCommand, BaseResponse>
    {
        public async Task<BaseResponse> Handle(DeleteSettingCommand request, CancellationToken cancellationToken = default)
        {
            var validationResult = await new Validator().ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return BaseResponse.Failed(validationResult.Errors);

            var setting = await GetAsync(request, cancellationToken);
            if (setting == null)
                return BaseResponse.Failed("Could not find setting");

            setting.Deleted = true;

            context.Settings.Update(setting);
            await context.SaveChangesAsync(cancellationToken);
            return BaseResponse.Succeeded();
        }

        private async Task<Setting?> GetAsync(DeleteSettingCommand request, CancellationToken cancellationToken = default)
        {
            return await (from s in context.Settings
                          where s.Id == request.Id
                          select s).FirstOrDefaultAsync(cancellationToken);
        }
    }

    private class Validator : AbstractValidator<DeleteSettingCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id is mandatory");
        }
    }
}