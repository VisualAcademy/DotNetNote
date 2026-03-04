namespace DotNetNote.Models
{
    public class WebCampSpeaker
    {
        public int Id { get; set; }                 // 번호
        public string Name { get; set; } = string.Empty;            // 이름
        public string Photo { get; set; } = string.Empty;           // 사진 파일명
        public string Title { get; set; } = string.Empty;           // 회사 정보, 직책
        public string Description { get; set; } = string.Empty;     // 소개
    }
}