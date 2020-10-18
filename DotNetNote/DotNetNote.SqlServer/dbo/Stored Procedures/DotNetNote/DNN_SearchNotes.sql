--[9] 게시판(DotNetNote)에서 데이터 검색 리스트: SearchNotes, NotesSearchList => NotesSearchList 저장 프로시저로 대체할 것
Create Procedure dbo.SearchNotes
    @Page Int,
    @SearchField NVarChar(25),
    @SearchQuery NVarChar(25)
As
    With DotNetNoteOrderedLists 
    As 
    (
        Select 
            [Id], [Name], [Email], [Title], [PostDate], 
            [ReadCount], [Ref], [Step], [RefOrder], [AnswerNum], 
            [ParentNum], [CommentCount], [FileName], [FileSize], 
            [DownCount], 
            ROW_NUMBER() Over (Order By Ref Desc, RefOrder Asc) 
            As 'RowNumber' 
        From Notes
        Where ( 
            Case @SearchField 
                When 'Name' Then [Name] 
                When 'Title' Then Title 
                When 'Content' Then Content 
                Else 
                @SearchQuery 
            End 
        ) Like '%' + @SearchQuery + '%'
    ) 
    Select 
        [Id], [Name], [Email], [Title], [PostDate], 
        [ReadCount], [Ref], [Step], [RefOrder], 
        [AnswerNum], [ParentNum], [CommentCount], 
        [FileName], [FileSize], [DownCount], 
        [RowNumber] 
    From DotNetNoteOrderedLists 
    Where RowNumber Between @Page * 10 + 1 And (@Page + 1) * 10  
    Order By Id Desc 
Go
