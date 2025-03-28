using System.Net;

namespace All.Utilities
{
    /// <summary>
    /// 이메일 본문을 생성하는 유틸리티 클래스입니다.
    /// 다양한 이메일 유형(문서 초대, 사건 할당 알림 등)에 대한 템플릿을 제공합니다.
    /// </summary>
    public class EmailContentGenerator
    {
        /// <summary>
        /// 샘플 이메일 본문을 생성하는 메서드입니다.
        /// 이메일 제목과 본문을 반환하는 예제용 템플릿입니다.
        /// </summary>
        /// <param name="recipientName">수신자 이름</param>
        /// <param name="emailAddress">수신자 이메일 주소</param>
        /// <param name="customMessage">사용자 정의 메시지</param>
        /// <returns>이메일 제목(subject)과 본문(body)</returns>
        public static (string subject, string body) GenerateSampleEmail(string recipientName, string emailAddress, string customMessage)
        {
            string subject = "Sample Email Notification";
            string encodedRecipientName = WebUtility.HtmlEncode(recipientName);
            string encodedEmailAddress = WebUtility.HtmlEncode(emailAddress);
            string encodedMessage = WebUtility.HtmlEncode(customMessage);

            string body = $@"
                <p>Hello {encodedRecipientName},</p>
                <p>This is a sample email template.</p>
                <p>{encodedMessage}</p>
                <p>If you have any questions, please contact us at <a href='mailto:{encodedEmailAddress}'>{encodedEmailAddress}</a>.</p>
                <p>Best Regards,<br/>Your Company</p>";

            return (subject, body);
        }
    }
}
