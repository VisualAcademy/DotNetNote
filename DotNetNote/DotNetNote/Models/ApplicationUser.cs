using Microsoft.AspNetCore.Identity;

namespace DotNetNote.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// 주소
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 성별
        /// </summary>
        public string Gender { get; set; }
    }
}
