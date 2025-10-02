using System.Collections.Immutable;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Azunt.Web.Infrastructure.MultiTenancy;

/// <summary>
/// 실제 테넌트 정책 로직을 구현하는 서비스.
/// appsettings.json:TenantPolicies 섹션을 읽어서
/// - 편집 권한 강제 허용 여부,
/// - 업로드 체크 우회 여부,
/// - 유저의 테넌트 매니저 여부
/// 를 판별합니다.
/// </summary>
public class TenantPolicyService : ITenantPolicyService
{
    // 편집 권한 강제 허용 테넌트 목록 (대소문자 무시)
    private readonly ImmutableHashSet<string> _editOverride;

    // 업로드 체크 우회 테넌트 목록 (대소문자 무시)
    private readonly ImmutableHashSet<string> _bypassUpload;

    // 매니저 역할명 접미사 (기본값: "Managers")
    private readonly string _managerRoleSuffix;

    // 특정 테넌트에 대한 커스텀 매니저 역할명 매핑
    // (예: { "Hawaso": "Hawaso-Admin" })
    private readonly IReadOnlyDictionary<string, string> _customManagerRoles;

    /// <summary>
    /// Options 바인딩을 통해 정책 데이터를 불러와 초기화합니다.
    /// </summary>
    public TenantPolicyService(IOptions<TenantPoliciesOptions> options)
    {
        var o = options.Value;
        // EditOverrideTenants → ImmutableHashSet으로 변환
        _editOverride = (o.EditOverrideTenants ?? new())
            .ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);

        // BypassUploadCheckTenants → ImmutableHashSet으로 변환
        _bypassUpload = (o.BypassUploadCheckTenants ?? new())
            .ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);

        // ManagerRoleSuffix → 설정값이 없으면 "Managers" 기본값 사용
        _managerRoleSuffix = string.IsNullOrWhiteSpace(o.ManagerRoleSuffix)
            ? "Managers"
            : o.ManagerRoleSuffix;

        // CustomManagerRoles → 없으면 빈 Dictionary
        _customManagerRoles = o.CustomManagerRoles ?? new Dictionary<string, string>();
    }

    /// <summary>
    /// 주어진 테넌트가 "편집 권한 강제 허용" 대상인지 여부를 반환합니다.
    /// </summary>
    public bool IsEditOverrideTenant(string? tenantName) =>
        !string.IsNullOrWhiteSpace(tenantName) && _editOverride.Contains(tenantName!);

    /// <summary>
    /// 주어진 테넌트가 "업로드 체크 우회" 대상인지 여부를 반환합니다.
    /// </summary>
    public bool IsBypassUploadCheckTenant(string? tenantName) =>
        !string.IsNullOrWhiteSpace(tenantName) && _bypassUpload.Contains(tenantName!);

    /// <summary>
    /// 현재 유저가 특정 테넌트의 매니저인지 판별합니다.
    /// 판별 순서:
    /// 1) CustomManagerRoles에 등록된 역할명 우선 확인
    /// 2) 없으면 "<TenantName><ManagerRoleSuffix>" 패턴으로 확인
    /// </summary>
    /// <param name="user">확인할 IdentityUser</param>
    /// <param name="isInRoleAsync">주어진 역할명을 체크할 비동기 함수 (예: UserManager.IsInRoleAsync)</param>
    /// <param name="knownTenantNames">검증 대상 테넌트 이름 목록</param>
    /// <returns>
    /// (isManager, resolvedTenantName)
    /// - isManager: 매니저 여부
    /// - resolvedTenantName: 매니저로 확인된 테넌트 이름
    /// </returns>
    public async Task<(bool isManager, string? resolvedTenantName)> IsTenantManagerAsync(
        IdentityUser user,
        Func<string, Task<bool>> isInRoleAsync,
        IEnumerable<string> knownTenantNames)
    {
        // 1) 커스텀 매니저 역할 우선 확인
        foreach (var (tenant, roleName) in _customManagerRoles)
        {
            if (await isInRoleAsync(roleName)) return (true, tenant);
        }

        // 2) 규칙 기반 역할명(<TenantName><Suffix>) 확인
        foreach (var t in knownTenantNames.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var roleName = $"{t}{_managerRoleSuffix}";
            if (await isInRoleAsync(roleName)) return (true, t);
        }

        // 매니저 아님
        return (false, null);
    }
}
