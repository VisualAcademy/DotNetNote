using System.ComponentModel.DataAnnotations;

namespace All.Models.RoleViewModels
{
    public class ApplicationRoleViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Please provide a role name.")]
        public string RoleName { get; set; }
    }
}
