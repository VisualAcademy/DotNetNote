--[1] 게시판(DotNetNote)용 테이블 설계
Create Table dbo.Notes
(
    Id              Int Identity(1, 1) Not Null Primary Key,    -- 번호
    Name            NVarChar(25) Not Null,                      -- 이름
    Email           NVarChar(100) Null,                         -- 이메일 
    Title           NVarChar(150) Not Null,                     -- 제목
    PostDate        DateTime Default GetDate() Not Null,        -- 작성일 
    PostIp          NVarChar(15) Null,                          -- 작성IP
    Content         NText Not Null,                             -- 내용
    Password        NVarChar(255) Null,                         -- 비밀번호
    ReadCount       Int Default 0,                              -- 조회수
    Encoding        NVarChar(10) Not Null,                      -- 인코딩(HTML/Text)
    Homepage        NVarChar(100) Null,                         -- 홈페이지
    ModifyDate      DateTime Null,                              -- 수정일 
    ModifyIp        NVarChar(15) Null,                          -- 수정IP

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
    Category        NVarChar(20) Default('Free') Null,          -- 카테고리(확장...) => '공지', '자유', '자료', '사진', ...

    -- 추가: 필요한 항목 추가
    Num             Int Null,                                   -- 번호(확장...)
    UserId          Int Null,                                   -- (확장...) 사용자 테이블 Id
    CategoryId      Int Null Default 0,                         -- (확장...) 카테고리 테이블 Id
    BoardId         Int Null Default 0,                         -- (확장...) 게시판(Boards) 테이블 Id
    ApplicationId    Int Null Default 0                          -- (확장용) 응용 프로그램 Id
)
Go
