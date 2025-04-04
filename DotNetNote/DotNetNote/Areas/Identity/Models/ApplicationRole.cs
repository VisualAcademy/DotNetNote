#nullable enable
using Microsoft.AspNetCore.Identity;

namespace DotNetNote.Areas.Identity.Models;

public class ApplicationRole : IdentityRole
{
    public string? Description { get; set; } = "";
}
