--[User][0][1] 회원관리를 위한 Users 테이블 생성

--[0] Users 테이블 생성
Create Table dbo.Users
(
    UID Int Identity(1, 1) Primary Key Not Null,
    UserID NVarChar(25) Not Null,
    -- 암호 필드를 20자에서 255자로 변경(암호화때문에)...
    [Password] NVarChar(255) Not Null
    -- 필요한 항목이 있으면, 언제든 추가
	, Username NVarChar(20) Null	-- 사용자 이름
)
Go

----[1] 입력 예시문
--Insert Into Users Values('admin', '1234')
----[2] 출력 예시문
--Select * From Users Order By UID Desc
----[3] 상세 예시문
--Select * From Users Where UID = 1
----[4] 수정 예시문
--Begin Tran
--	Update Users
--	Set	
--		UserID = 'redplus',
--		Password = '1234'
--	Where UID = 1
--Commit Tran
----[5] 삭제 예시문
--Delete Users Where UID = 1
----[6] 검색 예시문
--Select * From Users Where UserID Like '%red%'
--GO
