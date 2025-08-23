using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace ClearBudget.Infrastructure.Services.Communication;

public interface IEmailSender
{
    Task<bool> SendAsync(MimeMessage message, CancellationToken cancellationToken = default);
}

internal class EmailSender : IEmailSender, IDisposable
{
    private readonly ISmtpClient? _smtpClient;
    private readonly string _saveFilePath;
    private readonly bool _saveToServerEnabled;

    public EmailSender(IConfiguration configuration)
    {
        var settings = configuration.GetSection("SmtpSettings").Get<SmtpSettings>();
        if (settings == null)
            throw new NullReferenceException("Cannot fetch SMTP settings");

        _smtpClient = new SmtpClient();
        _smtpClient.Connect(settings.Host, settings.Port, SecureSocketOptions.StartTls);
        _smtpClient.Authenticate(settings.Username, settings.Password);

        _saveFilePath = settings.SavePath;
        _saveToServerEnabled = settings.SaveToServerEnabled;
    }

    public async Task<bool> SendAsync(MimeMessage message, CancellationToken cancellationToken = default)
    {
        if (_smtpClient == null || !_smtpClient.IsConnected)
            return false;

        await _smtpClient.SendAsync(message, cancellationToken);

        if (_saveToServerEnabled)
        {
            if (!Directory.Exists(_saveFilePath))
                Directory.CreateDirectory(_saveFilePath);

            await using var stream = File.Create($"{_saveFilePath}{DateTime.Now:yyyy-MMM-dd-hh-mm-ss}-{Guid.NewGuid()}.eml");
            await message.WriteToAsync(stream, cancellationToken);
        }
        return true;
    }

    public void Dispose()
    {
        if (_smtpClient == null) return;

        _smtpClient.Disconnect(true);
        _smtpClient.Dispose();
    }
}

public class SmtpSettings
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public bool SaveToServerEnabled { get; set; }
    public string SavePath { get; set; }
}
