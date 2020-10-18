CREATE TABLE [dbo].[DotNetNoteUsers]
(
	[Id] NVarChar(512) Not Null Primary Key,
	UserName NVarChar(256) Null,
	NormalizedUserName NVarChar(256) Null,
	PasswordHash NVarChar(Max) Null
)
Go
