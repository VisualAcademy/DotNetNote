/// <summary>
/// Board 모델 클래스(Board, BoardModel, BoardViewModel, BoardDto, ...)
/// BoardViews(Boards 테이블) 뷰와 일대일로 매핑되는 모델 클래스
/// </summary>
public class Board
{
    /// <summary> 
    /// 일련번호                                                                        
    /// </summary>
    public int BoardId { get; set; }

    /// <summary> 
    /// 게시판이름(별칭):Notice,Free,News...                                            
    /// </summary>
    public string BoardAlias { get; set; }

    /// <summary> 
    /// 게시판 제목 : 공지사항, 자유게시판                                               
    /// </summary>
    public string Title { get; set; }

    /// <summary> 
    /// 게시판 설명                                                                     
    /// </summary>
    public string Description { get; set; }

    /// <summary> 
    /// 회원제 연동시 시삽 권한 부여                                                     
    /// </summary>
    public int SysopUsername { get; set; }

    /// <summary> 
    /// 익명사용자(1) / 회원 전용(0)게시판 구분          
    /// </summary>
    public bool IsPublic { get; set; } = false;

    /// <summary> 
    /// 그룹으로 묶어서 관리하고자 할 때                                                 
    /// </summary>
    public string GroupName { get; set; }

    /// <summary> 
    /// 그룹내 순서                                                                     
    /// </summary>
    public int GroupOrder { get; set; }

    /// <summary> 
    /// 게시물 작성시 메일 전송 여부(현재는 사용 안함)...                                                  
    /// </summary>
    public bool MailEnable { get; set; } = false;

    /// <summary> 
    /// 전체 게시판 리스트에서 보일건지 여부(특정 게시판은 관리자만 볼 수 있도록)          
    //// </summary>
    public bool ShowList { get; set; } = true;

    /// <summary>
    /// Portal 메인 페이지에 요약 게시판으로 출력할 지 여부
    /// </summary>
    public bool MainShowList { get; set; } = true;

    /// <summary> 
    /// 게시판 스타일 : 기본(0), 강좌(1)                                                
    //// </summary>
    public int BoardStyle { get; set; } = 0; 
    
    /// <summary>
    /// 게시판 상단에 포함될 HTML 조각
    /// </summary>
    public string HeaderHtml { get; set; }

    /// <summary>
    /// 게시판 하단에 포함될 HTML 조각
    /// </summary>
    public string FooterHtml { get; set; }

    /// <summary>
    /// 게시판 생성 일시 보관
    /// </summary>
    public DateTimeOffset TimeStamp { get; set; }
}
