namespace DotNetNote.Models;

public class SignBase
{
    public int SignId { get; set; }         // 일련번호

    public string Username { get; set; } = string.Empty;    // 아이디

    public string Password { get; set; } = string.Empty;    // 암호

    public string Name { get; set; } = string.Empty;        // 이름

    public string Email { get; set; } = string.Empty;       // 이메일
}