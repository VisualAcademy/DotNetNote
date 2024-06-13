namespace DotNetNote.Models;

/// <summary>
/// Site 모델 클래스 === Sites 테이블
/// </summary>
public class Site
{
    /// <summary>
    /// 일련번호
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 제목, 사이트명, Name, Title 속성은 자주 사용되는 필수 속성들 중 하나   
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 사이트 URL
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// 사이트 간단 설명 
    /// </summary>
    public string Description { get; set; }
}
