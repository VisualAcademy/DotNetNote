namespace DotNetNote.Models;

/// <summary>
/// JSON Web Token 반환값
/// </summary>
public class JwtPacket
{
    public string Token { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Expiration { get; set; } = string.Empty;
}