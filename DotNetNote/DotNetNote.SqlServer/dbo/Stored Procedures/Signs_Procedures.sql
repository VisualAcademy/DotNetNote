--[Sign][0][2] Signs 관련 저장 프로시저 생성

--[1] 입력 저장 프로시저
Create Proc dbo.WriteSigns
	@UserId NVarChar(25),
	@Password NVarChar(20)
As
	Insert Into Signs Values(@UserId, @Password)
Go
--WriteSigns 'redplus', '1234'


--[2] 출력 저장 프로시저
--Create Proc dbo.ListSigns
--As
--	Select * From Signs Order By Id Desc
--Go
Create Proc dbo.ListSigns
As
	Select [Id], [UserId], [Password] From Signs Order By Id Desc
Go
--ListSigns


--[3] 상세 저장 프로시저
Create Proc dbo.ViewSigns
	@Id Int
As
	Select [Id], [UserId], [Password] From Signs Where Id = @Id
Go
--ViewSigns 5


--[4] 수정 저장 프로시저
Create Proc dbo.ModifySigns
	@UserId NVarChar(25),
	@Password NVarChar(20),
	@Id Int
As
	Begin Tran
		Update Signs
		Set	
			UserId = @UserId,
			[Password] = @Password
		Where Id = @Id
	Commit Tran
Go
--ModifySigns 'master', '1234', 2


--[5] 삭제 저장 프로시저
Create Proc dbo.DeleteSigns
	@Id Int
As
	Delete Signs Where Id = @Id
Go
--DeleteSigns 2


--[6] 검색 저장 프로시저
Create Proc dbo.SearchSigns
	@SearchField NVarChar(25),
	@SearchQuery NVarChar(25)
As
	Declare @strSql NVarChar(255)
	Set @strSql = '
		Select * From Signs 
		Where 
			' + @SearchField + ' Like ''%' + @SearchQuery + '%''
	'
	-- Print @strSql
	Exec(@strSql)
Go
--SearchSigns 'UserId', 'admin'
