namespace DotNetNote.Models
{
    public class SignBase
    {
        public int SignId { get; set; }         // 일련번호
        public string Username { get; set; }    // 아이디
        public string Password { get; set; }    // 암호
        public string Name { get; set; }        // 이름
        public string Email { get; set; }       // 이메일
    }
}
