namespace DotNetNote.Models
{
    /// <summary>
    /// JSON Web Token 반환값 
    /// </summary>
    public class JwtPacket
    {
        public string Token;
        public string Username;
        public string Expiration;
    }
}
