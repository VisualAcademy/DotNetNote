-- 창고
CREATE TABLE [dbo].[Warehouses]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),	-- 키
	[Name] NVarChar(200) Null,						-- 이름
	[Description] NVarChar(1000) Null,				-- 설명
	[Address] NVarChar(200) Null,					-- 주소
	[Phone] NVarChar(20) Null,						-- 전화번호
	-- 추가 항목
	Coordinate NVarChar(20) Null					-- 좌표(X, Y, Z): 층, 행, 열
)
Go
