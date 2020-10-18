--[Sign][0][1] 회원관리를 위한 Signs 테이블 생성

--[0] Signs 테이블 생성
CREATE TABLE [dbo].[Signs]
(
	[Id] Int Identity(1, 1) Primary Key Not Null,
	UserId NVarChar(25) Not Null,
	[Password] NVarChar(20) Not Null
    -- 필요한 항목이 있으면, 언제든 추가
)

----[1] 입력 예시문
--Insert Into Signs Values('admin', '1234')
----[2] 출력 예시문
--Select * From Signs Order By UID Desc
----[3] 상세 예시문
--Select * From Signs Where UID = 1
----[4] 수정 예시문
--Begin Tran
--	Update Signs
--	Set	
--		UserId = 'redplus',
--		Password = '1234'
--	Where UID = 1
--Commit Tran
----[5] 삭제 예시문
--Delete Signs Where UID = 1
----[6] 검색 예시문
--Select * From Signs Where UserId Like '%red%'
--GO
