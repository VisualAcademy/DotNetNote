-- 장비 테이블 
CREATE TABLE [dbo].[Devices]
(
	[Id] INT NOT NULL PRIMARY KEY Identity(1, 1),
	ModelName NVarChar(Max) Null,
	ModelType NVarChar(Max) Null,
	DeviceType NVarChar(Max) Null,
	DeviceId NVarChar(Max) Null,
	Maker NVarChar(Max) Null,
	UserRef NVarChar(Max) Null,					-- 홈페이지 
	UnitPrice BigInt Null,						-- 단가 

	-- 추가 
	CreatedDate DateTimeOffset Default(GetDate())
)
Go
