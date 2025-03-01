using System.Threading.Tasks;

namespace All.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
