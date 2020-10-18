--[6] 검색 결과의 레코드 수 반환: SearchNoteCount, NotesSearchCount 
Create Proc dbo.SearchNoteCount
    @SearchField NVarChar(25),
    @SearchQuery NVarChar(25)
As
    Set @SearchQuery = '%' + @SearchQuery + '%'

    Select Count(*)
    From Notes
    Where
    (
        Case @SearchField 
            When 'Name' Then [Name]
            When 'Title' Then Title
            When 'Content' Then Content
            Else @SearchQuery
        End
    ) 
    Like 
    @SearchQuery
Go
