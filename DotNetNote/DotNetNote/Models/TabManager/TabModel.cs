using System.Collections.Generic;

namespace DotNetNote.Models
{
    /// <summary>
    /// 탭(Tab) 모델 클래스: Tab, TabModel, TabViewModel, ... 
    /// </summary>
    public class TabModel
    {
        /// <summary>
        /// 일련번호+고유키
        /// </summary>
        public int TabId { get; set; }

        /// <summary>
        /// 보여지는 순서
        /// </summary>
        public int TabOrder { get; set; }

        /// <summary>
        /// 부모 메뉴 번호: 최상위(0)
        /// </summary>
        public int ParentId { get; set; } = 0;

        /// <summary>
        /// 탭 이름
        /// </summary>
        public string TabName { get; set; }

        /// <summary>
        /// 선택할 때 이동 경로
        /// </summary>
        public string TabPath { get; set; }

        /// <summary>
        /// 메뉴 표시 여부
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// 커뮤니티 번호(CommunityId, TabId, PortalId, ...)
        /// 관리자용 메뉴를 하나만 사용하려면 0으로 두고 사용 
        /// </summary>
        public int CommunityId { get; set; } = 0; 

        /// <summary>
        /// 게시판 링크에 대한 여부
        /// </summary>
        public bool IsBoard { get; set; }

        /// <summary>
        /// 링크의 target 속성 설정 값
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// 게시판 별칭
        /// </summary>
        public string BoardAlias { get; set; }

        /// <summary>
        /// 자식 메뉴 리스트
        /// </summary>
        public List<TabModel> Tabs { get; set; } = new List<TabModel>();

        /// <summary>
        /// ToString 메서드 오버라이드
        /// </summary>
        /// <returns>TabName</returns>
        public override string ToString()
        {
            return TabName;
        }
    }
}
