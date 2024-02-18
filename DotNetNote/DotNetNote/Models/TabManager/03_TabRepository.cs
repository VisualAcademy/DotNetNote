namespace DotNetNote.Models;

public class TabRepository : ITabRepository
{
    private IDbConnection db;
    private string _connectionString;

    public TabRepository()
    {
        _connectionString = "";
        db = new SqlConnection(_connectionString);
    }

    /// <summary>
    /// 전체 탭 리스트
    /// </summary>
    public List<TabModel> GetAll(int communityId = 0)
    {
        List<TabModel> tabs = new List<TabModel>();
        
        string sql = 
            "Select * From Tabs Where CommunityId = "
            + communityId.ToString()
            + " Order By TabOrder Asc, TabId Asc";

        var dt = db.Query<TabModel>(sql).ToList();

        var q =
            from dr in dt.AsEnumerable()
            select new TabModel
            {
                TabId = dr.TabId,
                TabOrder = dr.TabOrder,
                ParentId = dr.ParentId,
                TabName = dr.TabName,
                TabPath =
                    dr.TabPath != null
                    ?
                    dr.TabPath
                    :
                    "",

                IsVisible = dr.IsVisible,
                CommunityId = dr.CommunityId,
                IsBoard = dr.IsBoard,

                Target =
                    dr.Target != null
                    ?
                    dr.Target
                    :
                    "_self",

                BoardAlias =
                    dr.BoardAlias != null
                    ?
                    dr.BoardAlias
                    :
                    ""
            };

        if (q != null)
        {
            tabs = q.ToList();
        }

        return tabs;
    }

    public List<TabModel> GetAllByCommunityId(int communityId)
    {            
        string sql = 
            "Select * From Tabs Where CommunityId = "
            + communityId.ToString();

        var tabs = db.Query<TabModel>(sql).ToList();

        return GetMenuData(tabs, 0, communityId);
    }

    /// <summary>
    /// 커뮤니티별 IsVisible 속성이 참(1)인 메뉴만 읽어오기
    /// </summary>
    public List<TabModel> GetAllByCommunityId(
        int communityId, bool isVisible)
    {
        string sql = @"
                    Select * 
                    From Tabs 
                    Where 
                        IsVisible = " + ((isVisible) ? "1" : "0") + @"
                        And  
                        CommunityId = "
                + communityId.ToString();

        var tabs = db.Query<TabModel>(sql).ToList();

        return GetMenuData(tabs, 0, communityId);
    }

    public void UpdateModel(List<TabModel> lst)
    {
        for (int i = 0; i < lst.Count; i++)
        {
            Update(lst[i]);
        }
    }

    public TabModel Update(TabModel model)
    {
        var sql =
            " Update Tabs                  " +
            " Set                            " +
            "    TabName       =       @TabName,   " +
            "    TabOrder       =       @TabOrder,   " +
            "    TabPath       =       @TabPath,   " +
            "    IsVisible       =       @IsVisible,   " +
            "    IsBoard       =       @IsBoard,   " +
            "    Target       =       @Target,   " +
            "    BoardAlias       =       @BoardAlias " +
            " Where TabId = @TabId                 ";
        db.Execute(sql, model);
        return model;
    }

    private List<TabModel> GetMenuData(
        List<TabModel> Tabs, int parentId, int communityId)
    {
        List<TabModel> lst = new List<TabModel>();

        var q =
            from m in Tabs
            where m.ParentId == parentId && m.CommunityId == communityId
            orderby m.TabOrder
            select new TabModel
            {
                TabId = m.TabId,
                TabOrder = m.TabOrder,
                ParentId = m.ParentId,
                TabName = m.TabName,
                TabPath = m.TabPath,
                IsVisible = m.IsVisible,

                CommunityId = m.CommunityId,

                IsBoard = m.IsBoard,
                Target = m.Target,
                BoardAlias = m.BoardAlias,

                Tabs = (m.TabId != parentId)
                    ? GetMenuData(Tabs, m.TabId, communityId) : new List<TabModel>()
            };

        lst = q.ToList();

        return lst;
    }

    /// <summary>
    /// 메뉴 삭제
    /// </summary>
    public void Remove(int tabId)
    {
        string query = "Delete Tabs Where TabId = @TabId";
        db.Execute(query, new { TabId = tabId });
    }

    /// <summary>
    /// 메뉴 추가
    /// </summary>
    public TabModel Add(TabModel model)
    {
        string sql = @"
                Insert Into Tabs
                (
                    [TabOrder], 
                    [ParentId], 
                    [TabName], 
                    [TabPath], 
                    [IsVisible], 
                    [CommunityId], 
                    [IsBoard], 
                    [Target],
                    [BoardAlias]
                )
                Values
                (
                    @TabOrder, 
                    @ParentId, 
                    @TabName, 
                    @TabPath, 
                    @IsVisible, 
                    @CommunityId, 
                    @IsBoard, 
                    @Target,
                    @BoardAlias
                );

                Select Cast(SCOPE_IDENTITY() As Int);
            ";
        var TabId = db.Query<int>(sql, model).Single();
        model.TabId = TabId;
        return model;
    }

    public List<TabModel> GetMenusByParentId(int parentId)
    {
        string sql =
            "Select * From Tabs Where ParentId = @ParentId";
        return db.Query<TabModel>(
            sql, new { ParentId = parentId }).ToList();
    }

    public List<TabModel> GetMenusByParentId(
        int parentId, int communityId)
    {
        string sql = @"
                Select * From Tabs 
                Where 
                    ParentId = @ParentId 
                    And 
                    CommunityId = @CommunityId
            ";
        return db.Query<TabModel>(
            sql,
            new
            {
                ParentId = parentId,
                CommunityId = communityId
            }).ToList();
    }

    public int UpdateTabOrder(int parentId, int communityId)
    {
        string sql = "Select * From Tabs";
        int TabOrder = 0;

        if (parentId == 0)
        {
            // 기본은 상위 메뉴로 가정
            sql = @"
                    Select 
                        IsNull(Max(TabOrder), 0) As MaxTabOrder
                    From Tabs 
                    Where CommunityId = @CommunityId;
                ";
            TabOrder = db.Query<int>(sql,
                new { CommunityId = communityId }).SingleOrDefault();
        }
        else
        {
            // 서브 메뉴
            sql = @"
                    Declare @TabOrder Int;
                    Select @TabOrder = TabOrder
                    From Tabs 
                    Where TabId = @ParentId;

                    Update Tabs
                    Set
                        TabOrder = TabOrder + 1
                    Where 
                        CommunityId = @CommunityId
                        And
                        TabId > @TabOrder;

                    Select @TabOrder;
                ";
            TabOrder = db.Query<int>(sql,
                new
                {
                    ParentId = parentId,
                    CommunityId = communityId
                }).SingleOrDefault();
        }

        return (TabOrder + 1);
    }

}
