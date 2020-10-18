--[2] 게시판(DotNetNote)에서 데이터 출력: ListNotes => 업데이트된 NotesList 저장 프로시저 사용할 것 
Create Procedure dbo.ListNotes
    @Page Int
As
    With DotNetNoteOrderedLists 
    As 
    (
        Select 
            [Id], [Name], [Email], [Title], [PostDate], [ReadCount], 
            [Ref], [Step], [RefOrder], [AnswerNum], [ParentNum], 
            [CommentCount], [FileName], [FileSize], [DownCount], 
            ROW_NUMBER() Over (Order By Ref Desc, RefOrder Asc) 
            As 'RowNumber' 
        From Notes
    ) 
    Select * From DotNetNoteOrderedLists 
    Where RowNumber Between @Page * 10 + 1 And (@Page + 1) * 10
Go
