CREATE TABLE [dbo].[Posts]
(
	[PostId] INT NOT NULL PRIMARY KEY Identity(1, 1),
	Title NVarChar(255) Not Null,
	Content NVarChar(Max) Null,
	Created DateTime Default GetDate(),
	BlogId Int Default(0)
)
Go
