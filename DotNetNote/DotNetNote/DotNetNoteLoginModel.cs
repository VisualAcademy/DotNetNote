using System.ComponentModel.DataAnnotations;

namespace DotNetNoteCore
{
    public class DotNetNoteLoginModel
    {
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
