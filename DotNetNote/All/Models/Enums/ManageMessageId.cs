namespace All.Models.Enums
{
    /// <summary>
    /// 사용자 계정 관리 작업의 결과 메시지를 나타내는 열거형
    /// </summary>
    public enum ManageMessageId
    {
        AddPhoneSuccess,       // 전화번호 추가 성공
        AddLoginSuccess,       // 외부 로그인 추가 성공
        ChangePasswordSuccess, // 비밀번호 변경 성공
        SetTwoFactorSuccess,   // 2단계 인증 설정 성공
        SetPasswordSuccess,    // 비밀번호 설정 성공
        RemoveLoginSuccess,    // 외부 로그인 제거 성공
        RemovePhoneSuccess,    // 전화번호 제거 성공
        Error                  // 오류 발생
    }
}
