-- SQL Server를 사용하여 세션 개체를 저장하고자 할 때 사용되는 테이블 구조
CREATE TABLE [dbo].[SQLSessions](  
    [Id] [nvarchar](449) NOT NULL,  
    [Value] [varbinary](max) NOT NULL,  
    [ExpiresAtTime] [datetimeoffset](7) NOT NULL,  
    [SlidingExpirationInSeconds] [bigint] NULL,  
    [AbsoluteExpiration] [datetimeoffset](7) NULL,  
 CONSTRAINT [pk_Id] PRIMARY KEY CLUSTERED   
(  
    [Id] ASC  
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]  
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]  
GO

-- SQL Server를 사용하여 세션 개체를 저장하고자 할 때 사용되는 인덱스 구조
CREATE NONCLUSTERED INDEX [Index_ExpiresAtTime] ON [dbo].[SQLSessions]  
(  
    [ExpiresAtTime] ASC  
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
GO  
