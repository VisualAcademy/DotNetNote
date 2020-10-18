-- 명언(Maxim) 서비스
CREATE TABLE [dbo].[Maxims]
(
    [Id]            Int Primary Key Not Null Identity(1, 1),
    [Name]          NVarChar(25)    Not Null,                   -- 작성자
    [Content]       NVarChar(255)   Null,                       -- 명언 내용
    [CreationDate]  DateTime Default(GetDate())
)
Go
