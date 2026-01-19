namespace DotNetSale.Models;

/// <summary>
/// Category 클래스: Categories 테이블과 일대일
/// </summary>
public class Category
{
    /// <summary>
    /// 카테고리 번호
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// 카테고리 이름 (필수값)
    /// </summary>
    public required string CategoryName { get; set; }

    /// <summary>
    /// 부모 카테고리 번호
    /// </summary>
    public int SuperCategory { get; set; }

    /// <summary>
    /// 카테고리 보여지는 순서
    /// </summary>
    public int Align { get; set; }
}
