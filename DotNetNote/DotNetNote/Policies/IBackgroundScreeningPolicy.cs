namespace Azunt.Web.Policies;

public interface IBackgroundScreeningPolicy
{
    /// <summary>
    /// 지정한 Provider가 현재 tenantName에서 노출/사용 가능한지 여부
    /// </summary>
    bool IsProviderVisible(string provider, string? tenantName);
}
