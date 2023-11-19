namespace DotNetNote.Models;

/// <summary>
/// [2] 집합 클래스: 제품 리스트와 제품 수 
/// </summary>
public class GoodsSet
{
    /// <summary>
    /// 제품 리스트: 현재 페이지에 해당하는 제품 리스트
    /// </summary>
    public List<GoodsBase> Goods { get; set; }

    /// <summary>
    /// 제품 수: 현재 제품 관리의 전체 레코드 수 
    /// </summary>
    public int GoodsCount { get; set; }
}
