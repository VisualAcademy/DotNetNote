using System.ComponentModel.DataAnnotations;

namespace DotNetNote;

/// <summary>
/// View model used for user login.
/// </summary>
public class DotNetNoteLoginModel
{
    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}