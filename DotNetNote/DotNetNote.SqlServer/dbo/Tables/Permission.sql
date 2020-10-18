--[5] 사용자(또는 그룹/역할)에 대한 게시판 권한(퍼미션;Permission) 설정
CREATE TABLE [dbo].[Permission] 
(
    [TID]			[Int],						-- 테이블 일련번호
    [UID]			[Int],						-- 사용자(또는 그룹/역할) 일련번호
    [NoAccess]		[Bit] Default(0),			-- 접근거부
    [List]			[Bit] Default(1),			-- 리스트 보기
    [ReadArticle]	[Bit] Default(1),			-- 상세 보기
    [Download]		[Bit] Default(1),			-- 파일 다운로드
    [Write]			[Bit] Default(1),			-- 글쓰기
    [Upload]		[Bit] Default(1),			-- 업로드 
    [Extra]			[Bit] Default(0),			-- 관리
    [Admin]			[Bit] Default(0),			-- 모든권한
    [Comment]		[Bit] Default(1),			-- 댓글(댓글)

    Primary Key(TID, UID)
)
GO
