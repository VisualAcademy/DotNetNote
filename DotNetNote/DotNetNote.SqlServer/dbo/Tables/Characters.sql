CREATE TABLE [dbo].[Characters]
(
	[Id] INT NOT NULL PRIMARY KEY Identity(1, 1),
	[Username] NVarChar(25) Not Null,
	[HeroId] Int Not Null
)
Go

