//[User][2]
using System.ComponentModel.DataAnnotations;

namespace DotNetNote.Models
{
    public class UserViewModel
    {
        /// <summary>
        /// UID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// UserId
        /// </summary>
        [Display(Name = "아이디")]
        [Required(ErrorMessage = "아이디를 입력하시오.")]
        [StringLength(25, MinimumLength = 3,
            ErrorMessage = "아이디는 3자 이상 25자 이하로 입력하시오.")]
        public string UserId { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [Display(Name = "비밀번호")]
        [Required(ErrorMessage = "비밀번호를 입력하시오.")]
        [StringLength(20, MinimumLength = 6,
            ErrorMessage = "비밀번호는 6자 이상 20자 이하로 입력하시오.")]
        public string Password { get; set; }
    }
}
