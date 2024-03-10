using DotNetNoteCore.Services;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace DotNetNote.Services;

public class EmailManager : IEmailManager
{
    public EmailManager(IConfiguration configuration)
    {
        _adminName = configuration.GetSection("AdminName").Value;
        _adminEamil = configuration.GetSection("AdminEmail").Value;
        _sendGridKey = configuration.GetSection("AppKeys").GetSection("SendGridKey").Value;
    }

    private string _adminName;
    private string _adminEamil;
    private string _sendGridKey;

    public async Task SendEmailAsync(string email, string subject, string message, string recipient = "", string callbackUrl = "")
    {
        var client = new SendGridClient(_sendGridKey);

        var msg = new SendGridMessage()
        {
            From = new EmailAddress(_adminEamil, _adminName),
            Subject = subject,
            PlainTextContent = message,
        };

        msg.AddTo(new EmailAddress(email, recipient));
        var response = await client.SendEmailAsync(msg);
    }

    public async Task SendEmailCodeAsync(string email, string subject, string message, string recipient = "", string callbackUrl = "")
    {
        var client = new SendGridClient(_sendGridKey);

        var msg = new SendGridMessage()
        {
            From = new EmailAddress(_adminEamil, _adminName),
            Subject = subject,
            HtmlContent = message
        };

        msg.AddTo(new EmailAddress(email, recipient));
        var response = await client.SendEmailAsync(msg);
    }
}
