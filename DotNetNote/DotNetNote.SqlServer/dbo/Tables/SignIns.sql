-- 사용자 로그인 이력 테이블
CREATE TABLE [dbo].[SignIns] ( 
    [ID]               BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,   -- 기본 키(자동 증가)
    [DateTimeSignedIn] DATETIMEOFFSET(0) NOT NULL,                  -- 로그인 시각
    [UserID]           NVARCHAR(450) NULL,                          -- 사용자 ID
    [Email]            NVARCHAR(MAX) NOT NULL,                      -- 로그인에 사용된 이메일
    [FirstName]        NVARCHAR(MAX) NULL,                          -- 이름
    [LastName]         NVARCHAR(MAX) NULL,                          -- 성
    [Result]           NVARCHAR(MAX) NOT NULL,                      -- 로그인 결과(예: 성공/실패)
    [IPAddress]        NVARCHAR(MAX) NULL,                          -- 클라이언트 IP 주소
    [Note]             NVARCHAR(MAX) NULL,                          -- 비고/메모
    [TenantId]         BIGINT NULL,                                 -- 테넌트 ID (확장용)
    [TenantName]       NVARCHAR(255) NULL                           -- 테넌트 이름 (확장용)
);
