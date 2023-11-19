namespace DotNetNoteCore;

public class DotNetNoteUser
{
    public string Id { get; set; }

    public string UserName { get; set; }

    public string NormalizedUserName { get; set; }

    public string PasswordHash { get; set; }
}
