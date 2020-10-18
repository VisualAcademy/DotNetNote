-- 웹사이트의 회원별 포인트 관리
-- 종합 포인트
Create Table dbo.Points
(
    Id Int Identity(1, 1) Not Null Primary Key,         -- 일련번호
    UserId Int Not Null,                                -- 사용자ID
    Username NVarChar(25) Null,                         -- 사용자이름
    TotalPoint Int Default(0)                           -- 종합포인트
)
Go

-- 종합 포인트 로그
Create Table dbo.PointLogs
(
    Id Int Identity(1, 1) Not Null Primary Key,         -- 일련번호
    UserId Int Not Null,                                -- 사용자ID
    Username NVarChar(25) Null,                         -- 사용자이름
    NowPoint Int Default(1),                            -- 종합포인트
    Created   DatetimeOffset(7) Default(GetDate())      -- 등록일시
)
Go
