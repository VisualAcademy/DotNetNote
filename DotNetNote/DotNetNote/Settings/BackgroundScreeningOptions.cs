using System.Collections.Generic;

namespace Azunt.Web.Settings;

public sealed class BackgroundScreeningOptions
{
    public string ApiBaseUrl { get; set; } = "";
    public string ApiPrefix { get; set; } = "/v1";
    public string JwtToken { get; set; } = "";

    // key: ProviderName (예: "Azunt", "DevLec")
    public Dictionary<string, BackgroundProviderOptions> Providers { get; set; } = new();
}

public sealed class BackgroundProviderOptions
{
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// 허용 테넌트 화이트리스트.
    /// 빈 배열이면 "모두 허용"으로 정책에서 해석합니다.
    /// </summary>
    public List<string> AllowedTenants { get; set; } = new();
}
