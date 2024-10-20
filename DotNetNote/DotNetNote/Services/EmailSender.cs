namespace DotNetNote.Services;

// This class is used by the application to send email for account confirmation and password reset.
// For more details see https://go.microsoft.com/fwlink/?LinkID=532713
// Abstractions: Interfaces => IEmailSender
// Implementations: Classes => EmailSender, SendGridEmailSender, ...
public class EmailSender : IEmailSender
{
    private const string REPLY_TO_EMAIL = "support@hawaso.com";
    private const string REPLY_TO_NAME = "Hawaso Team";

    public Task SendEmailAsync(string email, string subject, string message) => Task.CompletedTask;
}
