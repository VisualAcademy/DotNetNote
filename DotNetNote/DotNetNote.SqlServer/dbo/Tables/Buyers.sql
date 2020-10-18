CREATE TABLE [dbo].[Buyers]
(
	[Id] INT NOT NULL PRIMARY KEY Identity(1, 1),
	[BuyerId] NVarChar(255) Null,
	[BuyerName] NVarChar(255) Null,
	[BuyerCode] NVarChar(255) Null
)
