--[3] SQL 저장 프로시저 6가지 작성

--[a] 메모 입력용 저장 프로시저
Create Procedure dbo.WriteMemo
(
	@Name NVarChar(25),
	@Email NVarChar(100),
	@Title NVarChar(150),
	@PostIp NVarChar(15)
)
As
	Insert Memos(Name, Email, Title, PostIp)
	Values(@Name, @Email, @Title, @PostIp)
Go

--[b] 메모 출력용 저장 프로시저
Create Proc dbo.ListMemo
As
	Select Num, Name, Email, Title, PostDate, PostIp
	From Memos Order By Num Desc
Go

--[c] 메모 상세 보기용 저장 프로시저
Create Proc dbo.ViewMemo
(
	@Num Int
)
As
	Select Num, Name, Email, Title, PostDate, PostIp 
	From Memos 
	Where Num = @Num
Go

--[d] 메모 데이터 수정용 저장 프로시저
Create Proc dbo.ModifyMemo
(
	@Name NVarChar(25),
	@Email NVarChar(100),
	@Title NVarChar(150),
	@Num Int 
)
As
Begin Transaction
	Update Memos 
	Set 
		Name = @Name, 
		Email = @Email,
		Title = @Title
	Where Num = @Num
Commit Transaction
Go

--[e] 메모 데이터 삭제용 저장 프로시저
Create Proc dbo.DeleteMemo
(
	@Num Int
)
As
	Delete Memos
	Where Num = @Num
Go

--[f] 메모 데이터 검색용 저장 프로시저(동적SQL 사용)
Create Proc dbo.SearchMemo
(
	@SearchField NVarChar(10),
	@SearchQuery NVarChar(50)
)
-- With Encryption -- 현재 SP문 암호화
As
	Declare @strSql NVarChar(250) -- 변수 선언
	Set @strSql = 
		'
		Select Num, Name, Email, Title, PostDate, PostIp 
		From Memos
		Where ' + @SearchField + ' Like 
			N''%' + @SearchQuery + '%''
		Order By Num Desc
		'
	--Print @strSql
	Exec (@strSql) --완성된 SQL문 실행
Go
