namespace DotNetNote.Models
{
    public sealed class ScreeningDemoVm
    {
        public string? TenantName { get; set; }      // 예: VisualAcademy, DotNetNote, Hawaso
        public string? PartnerName { get; set; }     // 현재 테넌트의 기본 파트너명 (있다면)
        public bool IsAdmin { get; set; }            // 관리자 여부(테스트용)
        public bool IsGlobalAdmin { get; set; }      // 글로벌 관리자 정책 여부(테스트용)
    }
}
