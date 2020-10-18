CREATE TABLE [dbo].[Questions]
(
	[Id] INT NOT NULL PRIMARY KEY Identity(1, 1),	-- 일련번호
	Title NVarChar(Max)	Not Null					-- 문제 제목(내용)
)
Go
