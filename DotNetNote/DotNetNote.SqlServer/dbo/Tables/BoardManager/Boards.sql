--게시판 관리용 테이블: 
--    다중으로 생성되는 모든 게시판 관련 정보는 Boards 테이블에서 관리됨
CREATE TABLE [dbo].[Boards]
(
    [TID]           [Int] IDENTITY(1,1) NOT NULL Primary Key,	--일련번호
    [BoardAlias]    [NVarChar](50) NULL,						--게시판이름(별칭):Notice,Free,News...
    [Title]	        [NVarChar](50) NULL,						--게시판 제목 : 공지사항, 자유게시판
    [Description]   [NVarChar](200) NULL,						--게시판 설명 
    --
    [SysopUID]      [Int] NULL DEFAULT ((0)),					--회원제 연동시 시삽 권한 부여
    [IsPublic]       [Bit] NULL DEFAULT ((1)),					--익명사용자(1) / 회원 전용(0)게시판 구분, C# : IsPublic으로 사용
    [GroupName]     [NVarChar](50) NULL,						--그룹으로 묶어서 관리하고자 할 때(Communities 테이블의 CommunityName 필드 참조), 추후 CommunityId도 따로 보관할 것...
    [GroupOrder]    [Int] NULL DEFAULT ((0)),					--그룹내 순서
    [MailEnable]    [Bit] NULL DEFAULT ((0)),					--게시물 작성시 메일 전송 여부(현재는 사용 안함)...

    -- 더 필요한 항목이 있으면 추가(확장)...
    [ShowList]		[Bit] NULL DEFAULT ((1)),					--전체 게시판 리스트에서 보일건지 여부(특정 게시판은 관리자만 볼 수 있도록)
    [BoardStyle]	[Int] NULL DEFAULT ((0)),					--게시판 스타일 : 기본(0), 강좌(1), 메모장(2), 이벤트(3) 

    -- 게시판 상단과 하단에 HTML 인클루드 용도
    [HeaderHtml]	[NVarChar](Max) NULL,						--게시판 상단 HTML 인클루드
    [FooterHtml]	[NVarChar](Max) NULL,						--게시판 하단 HTML 인클루드


    [MainShowList] Int Default(1)								-- 포탈에 표시할지 안할지 결정 : 0 : 숨김, 1 :  표시
    )
Go
