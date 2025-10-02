using Azunt.Web.Infrastructure.MultiTenancy;

namespace Azunt.Web.Infrastructure.Extensions;

/// <summary>
/// DI 컨테이너에 Azunt.Web 프로젝트에서 공통으로 사용하는
/// 서비스들을 등록하기 위한 확장 메서드 모음입니다.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Azunt.Web에서 필요한 공통 서비스들을 한번에 등록합니다.
    /// - TenantPoliciesOptions: appsettings.json → Options 패턴 바인딩
    /// - ITenantPolicyService: 싱글턴으로 정책 서비스 구현 등록
    /// - (확장 가능) Identity, DbContext, Repository 등도 이 지점에서 묶어서 등록
    /// </summary>
    /// <param name="services">DI 서비스 컬렉션</param>
    /// <param name="config">구성 객체(IConfiguration), appsettings.json 등에서 값 읽기</param>
    /// <returns>서비스 체이닝을 위해 IServiceCollection 그대로 반환</returns>
    public static IServiceCollection AddAzuntWeb(this IServiceCollection services, IConfiguration config)
    {
        // "TenantPolicies" 섹션을 TenantPoliciesOptions 클래스에 매핑
        services.Configure<TenantPoliciesOptions>(config.GetSection("TenantPolicies"));

        // 테넌트 정책 서비스 등록 (싱글턴)
        services.AddSingleton<ITenantPolicyService, TenantPolicyService>();

        // Identity/DbContext/Repositories 등 Azunt.Web 공통 서비스 등록 지점
        // services.AddDbContext<ApplicationDbContext>(...);
        // services.AddScoped<IDocumentRepository, DocumentRepository>();

        return services;
    }
}
