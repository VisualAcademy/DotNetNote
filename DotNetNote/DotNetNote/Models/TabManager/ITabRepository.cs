namespace DotNetNote.Models
{
    /// <summary>
    /// 탭 저장소에 대한 리포지토리 인터페이스
    /// </summary>
    public interface ITabRepository
    {
        /// <summary>
        /// 전체 탭 리스트
        /// </summary>
        /// <param name="communityId">커뮤니티 ID : 기본은 0로 두고 전체 메뉴 관리</param>
        /// <returns>특정 커뮤니티(응용 프로그램)별 관리자 메뉴(탭) 리스트</returns>
        List<TabModel> GetAll(int communityId = 0);
        
        List<TabModel> GetAllByCommunityId(int communityId);

        List<TabModel> GetAllByCommunityId(int communityId, bool isVisible);

        void UpdateModel(List<TabModel> lst);

        /// <summary>
        /// 삭제
        /// </summary>
        void Remove(int tabId);

        TabModel Add(TabModel model);

        List<TabModel> GetMenusByParentId(int parentId);

        List<TabModel> GetMenusByParentId(int parentId, int communityId);

        int UpdateTabOrder(int parentId, int communityId);
    }
}
