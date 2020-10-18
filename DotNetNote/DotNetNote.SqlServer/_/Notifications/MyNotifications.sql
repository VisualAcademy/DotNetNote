CREATE TABLE [dbo].[MyNotifications](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Timestamp] [datetimeoffset](7) NOT NULL,
	[Message] [nvarchar](max) NULL,
	[Type] [nvarchar](50) NULL,
	[Url] [nvarchar](max) NULL,
	--------------------------------------------------------
	UserId Int Null,
	IsComplete Bit Default(0) Not Null

 CONSTRAINT [PK_MyNotifications] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[MyNotifications] ADD  CONSTRAINT [DF_MyNotifications_Timestamp]  DEFAULT (sysdatetimeoffset()) FOR [Timestamp]
GO
