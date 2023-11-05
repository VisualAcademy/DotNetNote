namespace DotNetNote.Models
{
    /// <summary>
    /// Site 모델 클래스 === Sites 테이블
    /// </summary>
    public class Site
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
    }
}
