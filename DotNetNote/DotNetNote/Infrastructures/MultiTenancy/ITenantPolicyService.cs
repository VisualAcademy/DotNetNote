using Microsoft.AspNetCore.Identity;

namespace Azunt.Web.Infrastructure.MultiTenancy;

/// <summary>
/// 테넌트 정책을 정의하는 서비스 인터페이스.
/// - 특정 테넌트가 "편집 권한 강제 허용" 대상인지,
/// - "업로드 체크 우회" 대상인지,
/// - 또는 유저가 어떤 테넌트의 매니저 역할을 가지고 있는지 판별합니다.
/// </summary>
public interface ITenantPolicyService
{
    /// <summary>
    /// 주어진 테넌트가 "편집 권한 강제 허용" 목록에 속하는지 확인합니다.
    /// true이면 문서 소유자 여부와 상관없이 ViewBag.CanEdit = true 처리됩니다.
    /// </summary>
    bool IsEditOverrideTenant(string? tenantName);

    /// <summary>
    /// 주어진 테넌트가 "업로드 체크 우회" 대상인지 확인합니다.
    /// true이면 필수 업로드 파일 검증을 생략할 수 있습니다.
    /// </summary>
    bool IsBypassUploadCheckTenant(string? tenantName);

    /// <summary>
    /// 유저가 특정 테넌트의 매니저 역할을 가지고 있는지 판별합니다.
    /// - isInRoleAsync 콜백을 사용하여 실제 역할 보유 여부를 확인합니다.
    /// - knownTenantNames 컬렉션을 바탕으로 "<TenantName>Managers" 패턴
    ///   또는 CustomManagerRoles 설정을 기준으로 매니저 여부를 결정합니다.
    /// </summary>
    /// <param name="user">확인할 IdentityUser</param>
    /// <param name="isInRoleAsync">주어진 역할명으로 IsInRoleAsync를 호출하는 람다</param>
    /// <param name="knownTenantNames">알고 있는 테넌트 이름 목록 (문서/유저에서 수집)</param>
    /// <returns>
    /// (bool isManager, string? resolvedTenantName)
    /// - isManager: 매니저인지 여부
    /// - resolvedTenantName: 매니저로 확인된 테넌트 이름 (없으면 null)
    /// </returns>
    Task<(bool isManager, string? resolvedTenantName)> IsTenantManagerAsync(
        IdentityUser user,
        Func<string, Task<bool>> isInRoleAsync,
        IEnumerable<string> knownTenantNames);
}
