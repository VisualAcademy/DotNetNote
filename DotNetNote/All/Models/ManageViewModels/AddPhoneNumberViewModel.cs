using System.ComponentModel.DataAnnotations;

namespace All.Models.ManageViewModels
{
    /// <summary>
    /// 사용자가 전화번호를 추가할 때 입력하는 데이터를 저장하는 ViewModel 클래스입니다.
    /// </summary>
    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
    }
}
