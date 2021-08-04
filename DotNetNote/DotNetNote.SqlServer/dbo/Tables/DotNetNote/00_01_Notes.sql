--[0][1] Table: Notes(완성형 게시판) 테이블 설계 
--[!] 게시판 테이블 설계: Articles, Posts, Entries, Notes, Memos, (Basic+Upload+Reply) => DotNetNote/DotNetMemo
CREATE TABLE [dbo].[Notes]
(
    Id              Int Identity(1, 1) Not Null Primary Key,    -- 번호
    Email           NVarChar(100) Null,                         -- 이메일 
    Password        NVarChar(255) Null,                         -- 비밀번호
    ReadCount       Int Default 0,                              -- 조회수
    Encoding        NVarChar(10) Not Null,                      -- 인코딩(HTML/Text)
    Homepage        NVarChar(100) Null,                         -- 홈페이지
    ModifyDate      DateTime Null,                              -- 수정일 
    ModifyIp        NVarChar(15) Null,                          -- 수정IP

    --[0] 5W1H: 누가, 언제, 어디서, 무엇을, 어떻게, 왜
    [Name]          NVarChar(25) Not Null,                      -- [2][이름](작성자)
    PostDate        DateTime Default GetDate() Not Null,        -- 작성일 
    PostIp          NVarChar(15) Null,                          -- 작성IP
    [Title]         NVarChar(150) Not Null,                     -- [3][제목]
    [Content]       NText Not Null,                             -- [4][내용]__NVarChar(Max) => NText__
    Category        NVarChar(20) Default('Free') Null,          -- 카테고리(확장...) => '공지', '자유', '자료', '사진', ...

	--[2] 자료실 게시판 관련 주요 컬럼
    FileName        NVarChar(255) Null,                         -- 파일명
    FileSize        Int Default 0,                              -- 파일크기
    DownCount       Int Default 0,                              -- 다운수 

	--[3] 답변형 게시판 관련 주요 컬럼
    Ref             Int Not Null,                               -- 참조(부모글)
    Step            Int Default 0,                              -- 답변깊이(레벨)
    RefOrder        Int Default 0,                              -- 답변순서
    AnswerNum       Int Default 0,                              -- 답변수
    ParentNum       Int Default 0,                              -- 부모글번호

    CommentCount    Int Default 0,                              -- 댓글수

    -- 추가: 필요한 항목 추가
    Num             Int Null,                                   -- 번호(확장...)
    UserId          Int Null,                                   -- (확장...) 사용자 테이블 Id
    CategoryId      Int Null Default 0,                         -- (확장...) 카테고리 테이블 Id
    BoardId         Int Null Default 0,                         -- (확장...) 게시판(Boards) 테이블 Id
    ApplicationId    Int Null Default 0                         -- (확장용) 응용 프로그램 Id
)
Go
