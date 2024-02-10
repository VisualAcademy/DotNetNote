namespace DotNetNote.Models;

/// <summary>
/// [4] 리포지토리 클래스
/// 인터페이스 실제 내용 구현
/// </summary>
public class GoodsRepository : IGoodsRepository
{
    private readonly IConfiguration _configuration;
    private IDbConnection db;

    public GoodsRepository(IConfiguration configuration)
    {
        _configuration = configuration;
        string connectionString = _configuration.GetSection("ConnectionString").Value;
        db = new SqlConnection(connectionString);
    }

    /// <summary>
    /// 입력 메서드: Add, AddGoods, Write, Persist, Create 
    /// </summary>
    public GoodsBase AddGoods(GoodsBase model)
    {
        string sql = @"
                Insert Into Goods (GoodsName, GoodsDescription) Values (@GoodsName, @GoodsDescription);
                Select Cast(SCOPE_IDENTITY() As Int);
            ";
        model.GoodsId = db.Query<int>(sql, model).Single();
        return model;
    }

    /// <summary>
    /// 출력 메서드: GetAll, GetAllGoods, GetGoods, List 
    /// </summary>
    public List<GoodsBase> GetAllGoods()
    {
        string query = "Select * From Goods Order By GoodsId Desc";
        return db.Query<GoodsBase>(query).ToList();
    }

    /// <summary>
    /// 상세 메서드: GetById, GetGoodsById, GetGoods, FindGoods
    /// </summary>
    public GoodsBase GetGoodsById(int goodsId)
    {
        var q = "Select * From Goods Where GoodsId = @GoodsId";
        return db.Query<GoodsBase>(q, new { GoodsId = goodsId }).SingleOrDefault(); // null 반환 허용 
    }

    /// <summary>
    /// 수정 메서드: Update, UpdateGoods, Edit, Modify 
    /// </summary>
    public GoodsBase UpdateGoods(GoodsBase model)
    {
        // 인라인 SQL(Ad Hoc 쿼리) 또는 저장 프로시저 지정
        var query =
            "Update Goods                               " +
            "Set                                        " +
            "   GoodsName = @GoodsName,                 " +
            "   GoodsDescription = @GoodsDescription    " +
            "Where GoodsId = @GoodsId                   ";

        // 파라미터 추가 
        var parameters = new DynamicParameters();
        parameters.Add("@GoodsName",
            value: model.GoodsName,
            dbType: DbType.String,
            direction: ParameterDirection.Input);
        parameters.Add("@GoodsDescription",
            value: model.GoodsDescription,
            dbType: DbType.String,
            direction: ParameterDirection.Input);
        parameters.Add("@GoodsId",
            value: model.GoodsId,
            dbType: DbType.Int32,
            direction: ParameterDirection.Input);

        // 실행
        db.Execute(query, parameters, commandType: CommandType.Text);
        return model;
    }

    /// <summary>
    /// 삭제 메서드: Remove, RemoveGoods, Delete 
    /// </summary>
    public void RemoveGoods(int id)
    {
        var q = "Delete From Goods Where GoodsId = @GoodsId";
        db.Execute(q, new { GoodsId = id });
    }

    ///// <summary>
    /////  레코드 카운트 반환 메서드
    ///// </summary>
    //public int GetTotalRecordCountGoods()
    //{
    //    string countSql = "Select Count(*) From Goods";
    //    return db.Query<int>(countSql).FirstOrDefault(); 
    //}

    /// <summary>
    /// 페이징 메서드: 페이징 처리된 리스트와 전체 레코드 수 반환 
    /// </summary>
    public GoodsSet GetAllGoodsWithPaging(int pageNumber = 1, int pageSize = 10)
    {
        GoodsSet goodsSet = new GoodsSet();

        var q =
            "Select Count(*) From Goods; " +
            @"Select * From Goods Order By GoodsId Desc 
                    Offset ((@PageNumber - 1) * @PageSize) Rows 
                    Fetch Next @PageSize Rows Only; "; // Row_Number() -> Offset/Fetch

        using (var multiRecords = db.QueryMultiple(
            q, new { PageNumber = pageNumber, PageSize = pageSize }))
        {
            var count = multiRecords.Read<int>().SingleOrDefault();
            var list = multiRecords.Read<GoodsBase>().ToList();

            goodsSet.GoodsCount = count;
            goodsSet.Goods = list;
        }

        return goodsSet;
    }
}
