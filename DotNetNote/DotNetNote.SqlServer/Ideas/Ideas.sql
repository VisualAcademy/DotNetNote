--[1] 테이블: 아이디어 관리자 웹앱: 아이디어(생각) 저장 공간
CREATE TABLE [dbo].[Ideas]
(
	[Id] INT NOT NULL PRIMARY KEY Identity(1, 1),	-- 일련번호
	[Note] NVarChar(Max) Not Null					-- 노트
)
Go
