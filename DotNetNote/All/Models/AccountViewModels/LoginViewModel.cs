using System.ComponentModel.DataAnnotations;

namespace All.Models.AccountViewModels
{
    /// <summary>
    /// 로그인 화면에서 사용될 뷰 모델 정의
    /// </summary>
    public class LoginViewModel
    {
        [Required] // 필수 입력 필드
        [EmailAddress] // 이메일 형식인지 검증
        public string Email { get; set; } // 사용자 이메일

        [Required] // 필수 입력 필드
        [DataType(DataType.Password)] // 비밀번호 입력 필드로 설정
        public string Password { get; set; } // 사용자 비밀번호

        [Display(Name = "Remember me?")] // UI에서 표시될 이름 지정
        public bool RememberMe { get; set; } // 로그인 상태 유지 여부
    }
}
