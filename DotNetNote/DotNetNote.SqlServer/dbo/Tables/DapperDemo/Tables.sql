CREATE TABLE [dbo].[Tables]
(
	[Id] INT NOT NULL PRIMARY KEY Identity(1, 1),		-- 기본키(일련번호)
	Note NVarChar(Max) Null,							-- 비고
	TimeStamp DateTime Default(GetDate())				-- 날짜
)
Go
