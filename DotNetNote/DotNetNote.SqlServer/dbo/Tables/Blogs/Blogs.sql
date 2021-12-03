CREATE TABLE [dbo].[Blogs]
(
	[BlogId] INT NOT NULL PRIMARY KEY Identity(1, 1),
	Name NVarChar(50) Not Null,
	[Url] NVarChar(255) Null,
	Description NVarChar(Max) Null,
	BloggerName NVarChar(50) Null,						-- 블로거 이름
	DateCreated DateTime Default(GetDate())
)
Go
