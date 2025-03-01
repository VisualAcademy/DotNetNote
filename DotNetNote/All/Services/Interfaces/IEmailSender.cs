using System.Threading.Tasks;

namespace All.Services
{
    /// <summary>
    /// 이메일 전송 기능을 제공하는 인터페이스입니다.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// 비동기적으로 이메일을 전송합니다.
        /// </summary>
        /// <param name="email">수신자 이메일 주소</param>
        /// <param name="subject">이메일 제목</param>
        /// <param name="message">이메일 본문</param>
        /// <param name="isBodyHtml">이메일 본문을 HTML 형식으로 보낼지 여부(기본값: true)</param>
        Task SendEmailAsync(string email, string subject, string message, bool isBodyHtml = true);
    }

    public interface IMailchimpEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message, bool isBodyHtml = true);
    }
}
