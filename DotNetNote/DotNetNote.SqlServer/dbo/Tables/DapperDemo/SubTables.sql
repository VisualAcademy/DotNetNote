CREATE TABLE [dbo].[SubTables]
(
	[Id] INT NOT NULL PRIMARY KEY Identity(1, 1),		-- 기본키(일련번호)
	TableId Int Not Null,								-- 부모키 
	Note NVarChar(Max) Null								-- 비고
)
Go
