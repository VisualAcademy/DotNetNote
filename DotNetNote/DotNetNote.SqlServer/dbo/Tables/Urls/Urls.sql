-- 도메인 관리자
CREATE TABLE [dbo].[Urls]
(
	[Id] INT NOT NULL PRIMARY KEY Identity(1, 1),	-- 일련번호
	SiteUrl NVarChar(Max) Null,						-- 도메인
	Content NVarChar(Max) Null,						-- 설명
	UserName NVarChar(Max) Null,					-- 등록자
	Created DateTime Default(GetDate())				-- 등록일
)
Go
