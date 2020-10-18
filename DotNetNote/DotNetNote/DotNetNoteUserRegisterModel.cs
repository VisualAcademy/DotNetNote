using System.ComponentModel.DataAnnotations;

namespace DotNetNoteCore
{
    public class DotNetNoteUserRegisterModel
    {
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password")] 
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
