namespace DotNetNote.Models;

/// <summary>
/// Twelve 모델 클래스: 12개월 데이터 표시 및 요약
/// </summary>
public class Twelve
{
    /// <summary>
    /// 일련번호
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 참조번호 
    /// </summary>
    public int ParentId { get; set; }

    /// <summary>
    /// 월: 1부터 12까지
    /// </summary>
    public int MonthNumber { get; set; }

    /// <summary>
    /// 이익
    /// </summary>
    public double? Profit { get; set; }
}
