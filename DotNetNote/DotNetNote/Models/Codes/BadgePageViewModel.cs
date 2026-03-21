namespace VisualAcademy.Models.Codes
{
    // Badge 페이지에서 사용할 ViewModel
    public class BadgePageViewModel
    {
        public string Tenant { get; set; } = string.Empty;

        // QR 코드 1: 메인 포털
        public string CandidatePortalUrl { get; set; } = string.Empty;

        // QR 코드 2: 정보 변경 페이지
        public string ChangeOfInformationUrl { get; set; } = string.Empty;

        public string PageTitle { get; set; } = string.Empty;

        public string Heading { get; set; } = string.Empty;
    }
}