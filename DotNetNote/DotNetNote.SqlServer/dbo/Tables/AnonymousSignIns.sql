-- 익명 사용자 로그인 이력 테이블
CREATE TABLE [dbo].[AnonymousSignIns] (
    [ID]               BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,  -- 기본 키(자동 증가)
    [DateTimeSignedIn] DATETIMEOFFSET(0) NOT NULL,                 -- 로그인 시각
    [Email]            NVARCHAR(MAX) NOT NULL,                     -- 입력된 이메일
    [Result]           NVARCHAR(MAX) NOT NULL,                     -- 로그인 결과(성공/실패 등)
    [IPAddress]        NVARCHAR(MAX) NULL,                         -- 클라이언트 IP
    [Note]             NVARCHAR(MAX) NULL                          -- 비고
);