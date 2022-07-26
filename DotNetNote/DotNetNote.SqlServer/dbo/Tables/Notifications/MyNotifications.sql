CREATE TABLE [dbo].[MyNotifications] 
(
	[Id] [int] IDENTITY(1,1) Primary Key NOT NULL,
	[Timestamp] [datetimeoffset](7) NOT NULL Default(sysdatetimeoffset()),
	[Message] [nvarchar](max) NULL,
	[Type] [nvarchar](50) NULL,
	[Url] [nvarchar](max) NULL,
	--------------------------------------------------------
	UserId Int Null,
	IsComplete Bit Default(0) Not Null
)
GO
