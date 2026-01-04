using DotNetNoteCore.Services;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace DotNetNote.Services;

public class EmailManager : IEmailManager
{
    private readonly string _adminName;
    private readonly string _adminEmail;
    private readonly string _sendGridKey;

    public EmailManager(IConfiguration configuration)
    {
        // 필수 설정값이 없으면 바로 예외 던지도록 처리
        _adminName = configuration["AdminName"]
                     ?? throw new InvalidOperationException("AdminName 설정값이 없습니다.");
        _adminEmail = configuration["AdminEmail"]
                      ?? throw new InvalidOperationException("AdminEmail 설정값이 없습니다.");
        _sendGridKey = configuration["AppKeys:SendGridKey"]
                       ?? throw new InvalidOperationException("AppKeys:SendGridKey 설정값이 없습니다.");
    }

    public async Task SendEmailAsync(
        string email,
        string subject,
        string message,
        string recipient = "",
        string callbackUrl = "")
    {
        var client = new SendGridClient(_sendGridKey);

        var msg = new SendGridMessage
        {
            From = new EmailAddress(_adminEmail, _adminName),
            Subject = subject,
            PlainTextContent = message,
        };

        msg.AddTo(new EmailAddress(email, recipient));
        var response = await client.SendEmailAsync(msg);
        // 필요하다면 response.StatusCode 등 로깅 가능
    }

    public async Task SendEmailCodeAsync(
        string email,
        string subject,
        string message,
        string recipient = "",
        string callbackUrl = "")
    {
        var client = new SendGridClient(_sendGridKey);

        var msg = new SendGridMessage
        {
            From = new EmailAddress(_adminEmail, _adminName),
            Subject = subject,
            HtmlContent = message
        };

        msg.AddTo(new EmailAddress(email, recipient));
        var response = await client.SendEmailAsync(msg);
    }
}
