namespace DotNetNote.Models.ManageViewModels;

public class ExternalLoginsViewModel
{
    public IList<UserLoginInfo> CurrentLogins { get; set; } = new List<UserLoginInfo>();

    public IList<AuthenticationScheme> OtherLogins { get; set; } = new List<AuthenticationScheme>();

    public bool ShowRemoveButton { get; set; }

    public string? StatusMessage { get; set; }
}
