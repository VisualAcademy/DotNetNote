--[!] 메모 저장용 테이블
CREATE TABLE [dbo].[Memos] (
    [Num]      INT            IDENTITY (1, 1) NOT NULL,
    [Name]     NVARCHAR (25)  NOT NULL,
    [Email]    NVARCHAR (100) NULL,
    [Title]    NVARCHAR (150) NOT NULL,
    [PostDate] DATETIME       DEFAULT (getdate()) NULL,
    [PostIp]   NVARCHAR (15)  NULL,
    PRIMARY KEY CLUSTERED ([Num] ASC)
);








----[1] 한줄 메모장(Memos) 테이블 설계
--Create Table dbo.Memos
--(
--	Num			Int Identity(1, 1) Primary Key,		--번호
--	Name		NVarChar(25) Not Null,				--이름
--	Email		NVarChar(100) Null,					--이메일
--	Title		NVarChar(150) Not Null,				--메모
--	PostDate	DateTime Default(GetDate()),		--작성일
--	PostIp		NVarChar(15) Null					--IP주소
--)
--Go


----[2] SQL 예시문 6가지 작성

----[a] 입력 예시문: Insert문: FrmMemoWrite.aspx
--Insert Memos 
--Values
--(	
--	N'레드플러스', N'redplus@devlec.com', N'레드플러스입니다.'
--	, GetDate(), '127.0.0.1'
--)
--Go

----[b] 출력 예시문: Select문: FrmMemoList.aspx
--Select Num, Name, Email, Title, PostDate, PostIp 
--From Memos Order By Num Desc
--Go

----[c] 상세 예시문: Select문: FrmMemoView.aspx
--Select Num, Name, Email, Title, PostDate, PostIp 
--From Memos Where Num = 1
--Go

----[d] 수정 예시문: Update문: FrmMemoModify.aspx
--Begin Tran
--	Update Memos 
--	Set 
--		Name = N'백두산',
--		Email = N'admin@devlec.com',
--		Title = N'백두산입니다.',
--		PostIp = N'127.0.0.1' 
--	Where 
--		Num = 1
----RollBack Tran
--Commit Tran
--Go

----[e] 삭제 예시문: Delete문: FrmMemoDelete.aspx
--Begin Tran
--	Delete Memos
--	Where Num = 10
----RollBack Tran
--Commit Tran
--Go

----[f] 검색 예시문: Select문: FrmMemoSearch.aspx
---- Memos에서 이름이 레드플러스이거나 또는
---- 이메일에 'r'가 들어가는 자료의 모든 필드
---- 번호의 역순으로 검색
--Select Num, Name, Email, Title, PostDate
--From Memos
--Where
--	Name = '레드플러스'
--	Or
--	Email Like '%r%'
--Order By Num Desc
--Go


----[3] SQL 저장 프로시저 6가지 작성

----[a] 메모 입력용 저장 프로시저
--Create Procedure dbo.WriteMemo
--(
--	@Name NVarChar(25),
--	@Email NVarChar(100),
--	@Title NVarChar(150),
--	@PostIp NVarChar(15)
--)
--As
--	Insert Memos(Name, Email, Title, PostIp)
--	Values(@Name, @Email, @Title, @PostIp)
--Go

----[b] 메모 출력용 저장 프로시저
--Create Proc dbo.ListMemo
--As
--	Select Num, Name, Email, Title, PostDate, PostIp
--	From Memos Order By Num Desc
--Go

----[c] 메모 상세 보기용 저장 프로시저
--Create Proc dbo.ViewMemo
--(
--	@Num Int
--)
--As
--	Select Num, Name, Email, Title, PostDate, PostIp 
--	From Memos 
--	Where Num = @Num
--Go

----[d] 메모 데이터 수정용 저장 프로시저
--Create Proc dbo.ModifyMemo
--(
--	@Name NVarChar(25),
--	@Email NVarChar(100),
--	@Title NVarChar(150),
--	@Num Int 
--)
--As
--Begin Transaction
--	Update Memos 
--	Set 
--		Name = @Name, 
--		Email = @Email,
--		Title = @Title
--	Where Num = @Num
--Commit Transaction
--Go

----[e] 메모 데이터 삭제용 저장 프로시저
--Create Proc dbo.DeleteMemo
--(
--	@Num Int
--)
--As
--	Delete Memos
--	Where Num = @Num
--Go

----[f] 메모 데이터 검색용 저장 프로시저(동적SQL 사용)
--Create Proc dbo.SearchMemo
--(
--	@SearchField NVarChar(10),
--	@SearchQuery NVarChar(50)
--)
---- With Encryption -- 현재 SP문 암호화
--As
--	Declare @strSql NVarChar(250) -- 변수 선언
--	Set @strSql = 
--		'
--		Select Num, Name, Email, Title, PostDate, PostIp 
--		From Memos
--		Where ' + @SearchField + ' Like 
--			N''%' + @SearchQuery + '%''
--		Order By Num Desc
--		'
--	--Print @strSql
--	Exec (@strSql) --완성된 SQL문 실행
--Go
