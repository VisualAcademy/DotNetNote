namespace DotNetNote.Models.Companies;

/// <summary>
/// Company, CompanyDomain, ...
/// </summary>
public class CompanyModel
{
    public long Id { get; set; }     // 테이블 PK (BIGINT, Identity)
    public string Name { get; set; } // 회사 이름(필수)
}
