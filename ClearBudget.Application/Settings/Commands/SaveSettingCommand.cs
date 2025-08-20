using ClearBudget.Database;
using ClearBudget.Database.Entities.Settings;
using ClearBudget.Infrastructure.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClearBudget.Application.Settings.Commands;

public class SaveSettingCommand : IRequest<BaseResponse>
{
    public Guid? Id { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }

    internal class Handler(IDbContext context) : IRequestHandler<SaveSettingCommand, BaseResponse>
    {
        public async Task<BaseResponse> Handle(SaveSettingCommand request, CancellationToken cancellationToken = default)
        {
            var validationResult = await new Validator().ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return BaseResponse.Failed(validationResult.Errors);

            return request.Id.HasValue && request.Id != Guid.Empty
                ? await UpdateAsync(request, cancellationToken)
                : await AddAsync(request, cancellationToken);
        }

        private async Task<BaseResponse> AddAsync(SaveSettingCommand request, CancellationToken cancellationToken = default)
        {
            var setting = new Setting
            {
                Key = request.Key,
                Value = request.Value
            };

            context.Settings.Add(setting);
            await context.SaveChangesAsync(cancellationToken);
            return BaseResponse.Succeeded();
        }

        private async Task<BaseResponse> UpdateAsync(SaveSettingCommand request, CancellationToken cancellationToken = default)
        {
            var setting = await GetAsync(request, cancellationToken);
            if (setting == null)
                return BaseResponse.Failed("Setting could not be found");

            setting.Key = request.Key;
            setting.Value = request.Value;

            context.Settings.Update(setting);
            await context.SaveChangesAsync(cancellationToken);
            return BaseResponse.Succeeded();
        }

        private async Task<Setting?> GetAsync(SaveSettingCommand request, CancellationToken cancellationToken = default)
        {
            return await (from s in context.Settings
                          where s.Id == request.Id.Value
                          select s).FirstOrDefaultAsync(cancellationToken);
        }
    }

    private class Validator : AbstractValidator<SaveSettingCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Key).NotEmpty().WithMessage("Key is mandatory");
            RuleFor(x => x.Value).NotEmpty().WithMessage("Value is mandatory");
        }
    }
}