using Azunt.Web.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace Azunt.Web.Policies;

public sealed class BackgroundScreeningPolicy : IBackgroundScreeningPolicy
{
    private readonly IOptionsSnapshot<BackgroundScreeningOptions> _opts;

    public BackgroundScreeningPolicy(IOptionsSnapshot<BackgroundScreeningOptions> opts)
        => _opts = opts;

    public bool IsProviderVisible(string provider, string? tenantName)
    {
        if (string.IsNullOrWhiteSpace(provider)) return false;

        var providers = _opts.Value.Providers;

        // 대/소문자 무시로 안전하게 조회
        var p = providers
            .FirstOrDefault(kv => string.Equals(kv.Key, provider, StringComparison.OrdinalIgnoreCase))
            .Value;

        if (p is null || !p.Enabled) return false;

        // AllowedTenants 비었으면 모두 허용
        if (p.AllowedTenants is null || p.AllowedTenants.Count == 0)
            return true;

        if (string.IsNullOrWhiteSpace(tenantName))
            return false;

        return p.AllowedTenants.Any(t =>
            string.Equals(t, tenantName, StringComparison.OrdinalIgnoreCase));
    }
}
