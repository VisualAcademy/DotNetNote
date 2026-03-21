namespace VisualAcademy.Models.Configuration
{
    // 테넌트 목록 바인딩용 클래스
    public class TenantSettings
    {
        public string[] Tenants { get; set; } = System.Array.Empty<string>();
    }
}