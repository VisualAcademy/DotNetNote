-- 사용자 로그인 이력 테이블
CREATE TABLE [dbo].[SignIns] ( 
    [ID] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY, -- 기본 키(자동 증가)
    [DateTimeSignedIn] [datetimeoffset](0) NOT NULL,  -- 로그인 시각
    [UserID] [nvarchar](450) NULL,                    -- 사용자 ID
    [Email] [nvarchar](max) NOT NULL,                 -- 로그인에 사용된 이메일
    [FirstName] [nvarchar](max) NULL,                 -- 이름
    [LastName] [nvarchar](max) NULL,                  -- 성
    [Result] [nvarchar](max) NOT NULL,                -- 로그인 결과(예: 성공/실패)
    [IPAddress] [nvarchar](max) NULL,                 -- 클라이언트 IP 주소
    [Note] [nvarchar](max) NULL                       -- 비고/메모
);
