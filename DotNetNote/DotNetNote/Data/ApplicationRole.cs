#nullable enable
using DotNetNote;
using Microsoft.AspNetCore.Identity;

namespace DotNetNote.Data;

public class ApplicationRole : IdentityRole
{
    public string? Description { get; set; } = "";
}
