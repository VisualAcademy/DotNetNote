--[User][0][2] Users 관련 저장 프로시저 생성

--[1] 입력 저장 프로시저
Create Proc dbo.WriteUsers
	@UserID NVarChar(25),
	@Password NVarChar(255)
As
	Insert Into Users (UserID, Password) Values(@UserID, @Password)
Go
--WriteUsers 'redplus', '1234'


--[2] 출력 저장 프로시저
--Create Proc dbo.ListUsers
--As
--	Select * From Users Order By UID Desc
--Go
Create Proc dbo.ListUsers
As
	Select [UID], [UserID], [Password] From Users Order By UID Desc
Go
--ListUsers


--[3] 상세 저장 프로시저
Create Proc dbo.ViewUsers
	@UID Int
As
	Select [UID], [UserID], [Password] From Users Where UID = @UID
Go
--ViewUsers 5


--[4] 수정 저장 프로시저
Create Proc dbo.ModifyUsers
	@UserID NVarChar(25),
	@Password NVarChar(255),
	@UID Int
As
	Begin Tran
		Update Users
		Set	
			UserID = @UserID,
			[Password] = @Password
		Where UID = @UID
	Commit Tran
Go
--ModifyUsers 'master', '1234', 2


--[5] 삭제 저장 프로시저
Create Proc dbo.DeleteUsers
	@UID Int
As
	Delete Users Where UID = @UID
Go
--DeleteUsers 2


--[6] 검색 저장 프로시저
Create Proc dbo.SearchUsers
	@SearchField NVarChar(25),
	@SearchQuery NVarChar(25)
As
	Declare @strSql NVarChar(255)
	Set @strSql = '
		Select * From Users 
		Where 
			' + @SearchField + ' Like ''%' + @SearchQuery + '%''
	'
	-- Print @strSql
	Exec(@strSql)
Go
--SearchUsers 'UserID', 'admin'
