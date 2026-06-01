CREATE TABLE [dbo].[LogBases]
(
    [Id] INT Identity(1, 1) NOT NULL PRIMARY KEY,               -- 일련번호
    [Message] NVARCHAR(MAX) NOT NULL,                           -- 로그 메시지
    [Timestamp] DATETIMEOFFSET(7) DEFAULT (SYSDATETIMEOFFSET()) -- 시간
)
GO
