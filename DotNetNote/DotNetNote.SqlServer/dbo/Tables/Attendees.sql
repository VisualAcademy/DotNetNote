--[1] 테이블 설계: 참여자(Attendees) 리스트(Camps, Talks 등을 참조)
CREATE TABLE [dbo].[Attendees]
(
    [Id] INT NOT NULL PRIMARY KEY Identity(1, 1),	-- 일련번호
    [UID] Int Not Null,								-- 사용자 UID
    [UserID] NVarChar(25) Not Null,					-- 사용자 ID
    [Name] NVarChar(25) Not Null,					-- 참석자 이름, 닉네임
    [CreationDate] DateTime Default(GetDate())		-- 등록일
)
Go
