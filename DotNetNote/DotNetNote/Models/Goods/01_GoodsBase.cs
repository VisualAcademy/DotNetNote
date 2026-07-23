namespace DotNetNote.Models;

/// <summary>
/// [1] 모델 클래스
/// Goods 테이블과 일대일로 매핑되는 모델 클래스:
/// Goods, GoodsModel, GoodsViewModel, GoodsBase, GoodsDto, GoodsEntity, ...
/// </summary>
public class GoodsBase
{
    public int GoodsId { get; set; }

    public string GoodsName { get; set; } = string.Empty;

    public string GoodsDescription { get; set; } = string.Empty;

    public DateTimeOffset CreatedDate { get; set; }
}