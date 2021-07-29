-- 로그인 실패 시도 등의 정보 저장: 계정 잠금 기능
CREATE TABLE [dbo].[UserLogs]
(
	-- 일련번호
	[Id] INT NOT NULL PRIMARY KEY Identity(1, 1),		
	-- 사용자 아이디
	Username NVarChar(50) Not Null,						
	-- 로그인 실패 카운트
	FailedPasswordAttemptCount Int Default(0),			
	-- 로그인 실패 처음 생성일
	FailedPasswordAttemptWindowStart 
		DateTime Default(GetDate())						
)
Go
