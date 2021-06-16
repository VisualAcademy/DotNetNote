--[?] ASP.NET & Core를 다루는 기술 책/강의 36강 데모 예제
--[1] 테이블 설계: 참여자(Attendees) 리스트
CREATE TABLE [dbo].[CommunityCampJoinMembers]
(
    [Id] INT NOT NULL PRIMARY KEY Identity(1, 1),   -- 일련번호
    [CommunityName] NVarChar(25) Not Null,          -- 커뮤니티명
    [Name] NVarChar(25) Not Null,                   -- 참석자 이름
    [Mobile] NVarChar(30) Not Null,                 -- 휴대폰 번호
    [Email] NVarChar(100) Not Null,                 -- 이메일 주소
    [Size] NVarChar(10) Not Null Default('L'),      -- 티셔츠 기념품 사이즈
    [CreationDate] DateTime Default(GetDate())      -- 등록일
)
Go
