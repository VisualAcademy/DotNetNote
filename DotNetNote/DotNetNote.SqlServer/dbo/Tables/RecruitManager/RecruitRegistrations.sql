-- 모집 신청 테이블
CREATE TABLE [dbo].[RecruitRegistrations]
(
    [Id] INT Identity(1, 1) NOT NULL PRIMARY KEY,	-- 일련번호

    [RecruitSettingId] Int Null,					-- RecruitSettings.Id

    BoardName		NVarChar(50) Null,	-- 게시판이름(확장): Notice, Free, Qna
    BoardNum		Int Null,			-- 해당 게시판의 게시물 번호
    BoardTitle		NVarChar(150) Null,	-- 게시판 제목(이벤트 제목)

    CreationDate	DateTimeOffset(7) Default(GetDate()),	-- 생성일 

    UserId			Int				Null,		-- 사용자 Id
    Username		NVarChar(25) Not Null,		-- 사용자 아이디
    NickName		NVarChar(100) Null			-- 닉네임 
)
Go
