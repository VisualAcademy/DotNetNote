-- 로그(Log) 관리
-- 로그(Logs) 테이블 생성
Create Table Logs
(
	[Id]				Int Identity(1, 1) Primary Key Not Null,	-- 일련번호
	[Note]				NVarChar(Max) Null,							-- 비고

	[Application]		NVarChar(50) Null,	-- 응용 프로그램: 게시판 관리, ...
	[Logger]			NVarChar(255) Null,	-- 사용자 정보(로그인 사용 아이디)
	[LogEvent]			NVarChar(Max) Null,	-- 로그 이벤트(사용자 정의이벤트 ID)
	[Message]			NVarChar(Max) Null,	-- 로그 메시지 
	[MessageTemplate]	NVarChar(Max) Null,	-- 로그 메시지에 대한 템플릿
	[Level]				NVarChar(128),		-- 로그 레벨(정보, 에러, 경고)
	[TimeStamp]			DateTimeOffset(7) Not Null,		-- 로그 발생 시간
	[Exception]			NVarChar(Max) Null,				-- 예외 메시지 
	[Properties]		Xml Null,					-- 기타 여러 속성들 XML 저장
	[Callsite]			NVarChar(Max) Null,			-- 호출사이트
	[IpAddress]			NVarChar(20) Null			-- IP 주소
)
Go
