namespace DotNetNote.Models
{
    public sealed class UserInfo
    {
        public required string UserId { get; set; }
        public required string Name { get; set; }

        public const string UserIdClaimType = "sub";
        public const string NameClaimType = "name";
    }
}
