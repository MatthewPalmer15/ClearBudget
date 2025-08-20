namespace ClearBudget.Application.Settings.Models;

public class GetSettingsResult
{
    public List<Setting> Settings { get; set; }

    public class Setting
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}