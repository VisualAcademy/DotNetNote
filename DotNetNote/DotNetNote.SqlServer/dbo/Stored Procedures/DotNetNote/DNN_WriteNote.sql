--[1] 게시판(DotNetNote)에 글을 작성 : WriteNote
Create Proc dbo.WriteNote
    -- 5W1H
    @Name       NVarChar(25), 
    @PostIp     NVarChar(15), 
    @Title      NVarChar(150), 
    @Content    NText, 
	@Category   NVarChar(10), 

    @Email      NVarChar(100), 
    @Password   NVarChar(255), 
    @Encoding   NVarChar(10), 
    @Homepage   NVarChar(100),
    @FileName   NVarChar(255),
    @FileSize   Int
As
	--[A] Ref 열에 일련번호 생성(현재 저장된 Ref 중 가장 큰 값에 1을 더해서 증가) 및 그룹화
    Declare @MaxRef Int
    Select @MaxRef = Max(IsNull(Ref, 0)) From Notes
 
    If @MaxRef Is Null Or @MaxRef = 0
        Set @MaxRef = 1 -- 테이블 생성 후 처음만 비교
    Else
        Set @MaxRef = @MaxRef + 1

	--[B] 만들어진 데이터를 저장하기 
    Insert Notes
    (
        Name, Email, Title, PostIp, Content, Password, Encoding, 
        Homepage, Ref, FileName, FileSize,
		Category 
    )
    Values
    (
        @Name, @Email, @Title, @PostIp, @Content, @Password, @Encoding, 
        @Homepage, @MaxRef, @FileName, @FileSize,
		@Category
    )
Go
