-- 할일 관리 응용 프로그램 만들기
CREATE TABLE [dbo].[Tasks]
(
    [Id]            INT             NOT NULL 
        PRIMARY KEY Identity(1, 1),                         -- 번호
    Title           NVarChar(140)   Not Null,               -- 할일 제목
    IsCompleted     BIT,                                    -- 완료 여부
    CreationDate    DateTime        Default(GetDate())      -- 등록일
)
Go
