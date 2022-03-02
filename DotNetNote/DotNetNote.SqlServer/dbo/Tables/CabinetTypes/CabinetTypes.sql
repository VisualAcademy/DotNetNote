-- 캐비넷 타입 관리용 테이블 
CREATE TABLE [dbo].[CabinetTypes] (
    [Id]             BIGINT         IDENTITY (1, 1) NOT NULL Primary Key,
    [Identification] NVARCHAR (MAX) NULL, -- 이름
    [Show]           BIT            NULL, -- 표시 여부 
    [Adjusted]       BIT            NOT NULL -- 백업 여부 
);
