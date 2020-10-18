--[!] 권한 입력 저장 프로시저
Create Proc dbo.AddPermission
    @TID Int, 
    @UID Int, 
    @NoAccess Bit, 
    @List Bit, 
    @ReadArticle Bit, 
    @Download Bit, 
    @Write Bit, 
    @Upload Bit, 
    @Extra Bit, 
    @Admin Bit,
    @Comment Bit
As
    Insert Into Permission (
		TID, UID, NoAccess, List, ReadArticle, Download, Write, Upload, Extra, Admin, Comment
	) 
    Values (
		@TID, @UID, @NoAccess, @List, @ReadArticle, @Download, @Write, @Upload, @Extra, @Admin, @Comment
	)	
Go
