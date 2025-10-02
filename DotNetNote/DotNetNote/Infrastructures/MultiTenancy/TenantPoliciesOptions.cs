namespace Azunt.Web.Infrastructure.MultiTenancy;

/// <summary>
/// Tenant policy settings bound from appsettings.json:TenantPolicies
/// </summary>
public class TenantPoliciesOptions
{
    /// <summary>
    /// Tenants that always have edit permission,
    /// regardless of document ownership or manager role.
    /// Example: ["VisualAcademy", "DotNetNote"]
    /// </summary>
    public List<string> EditOverrideTenants { get; set; } = new();

    /// <summary>
    /// Tenants allowed to bypass upload validation checks
    /// (e.g., skip required file upload verification before submit).
    /// Example: ["DotNetNote", "Hawaso"]
    /// </summary>
    public List<string> BypassUploadCheckTenants { get; set; } = new();

    /// <summary>
    /// Default suffix used for tenant manager roles.
    /// For example, if set to "Managers", "VisualAcademy" → "VisualAcademyManagers".
    /// </summary>
    public string ManagerRoleSuffix { get; set; } = "Managers";

    /// <summary>
    /// Custom manager role names for specific tenants that
    /// override the default <TenantName><ManagerRoleSuffix> pattern.
    /// Example: { "Hawaso": "Hawaso-Admin" }
    /// </summary>
    public Dictionary<string, string> CustomManagerRoles { get; set; } = new();
}
