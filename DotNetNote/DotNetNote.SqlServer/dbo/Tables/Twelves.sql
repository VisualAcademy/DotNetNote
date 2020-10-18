-- Twelves 테이블: 12개월 데이터 표시 및 요약
CREATE TABLE [dbo].[Twelves]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	ParentId Int Null, 
    [MonthNumber] INT NOT NULL, 
	[Profit]	Float Null,
)
Go
