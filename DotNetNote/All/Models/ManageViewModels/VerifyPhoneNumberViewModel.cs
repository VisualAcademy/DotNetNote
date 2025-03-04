using System.ComponentModel.DataAnnotations;

namespace All.Models.ManageViewModels
{
    /// <summary>
    /// 사용자가 받은 인증 코드를 입력하여 전화번호를 검증하는 ViewModel 클래스입니다.
    /// </summary>
    public class VerifyPhoneNumberViewModel
    {
        [Required]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
    }
}
