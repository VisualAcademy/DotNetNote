namespace DotNetNote.Models
{
    public class WebCampSpeaker
    {
        public int Id { get; set; }                 // 번호
        public string Name { get; set; }            // 이름
        public string Photo { get; set; }           // 사진 파일명
        public string Title { get; set; }           // 회사 정보, 직책
        public string Description { get; set; }     // 소개
    }
}
