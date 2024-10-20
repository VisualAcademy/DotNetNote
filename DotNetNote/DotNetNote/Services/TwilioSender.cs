namespace DotNetNote.Services
{
    public interface ITwilioSender
    {
        Task SendSmsAsync(string phoneNumber, string message);
    }

    public class TwilioSender : ITwilioSender
    {
        public Task SendSmsAsync(string phoneNumber, string message)
        {
            throw new NotImplementedException();
        }
    }
}
