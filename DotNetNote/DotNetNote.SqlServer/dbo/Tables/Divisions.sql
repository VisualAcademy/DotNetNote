--[0][0] 디비전: Divisions 
CREATE TABLE [dbo].[Divisions]
(
    [Id]        BIGINT             IDENTITY (1, 1) NOT NULL PRIMARY KEY,    -- 디비전 고유 아이디, 자동 증가
    [Active]    BIT                DEFAULT ((1)) NULL,                      -- 활성 상태 표시, 기본값 1 (활성)
    [CreatedAt] DATETIMEOFFSET NULL DEFAULT SYSDATETIMEOFFSET(),            -- 레코드 생성 시간
    [CreatedBy] NVARCHAR (255)     NULL,                                    -- 레코드 생성자 이름
    [Name]      NVARCHAR (MAX)     NULL                                     -- 이름
);