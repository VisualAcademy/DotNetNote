using System.Threading.Tasks;

namespace All.Services
{
    public interface ITwilioSender
    {
        Task SendSmsAsync(string phoneNumber, string message);
    }
}
