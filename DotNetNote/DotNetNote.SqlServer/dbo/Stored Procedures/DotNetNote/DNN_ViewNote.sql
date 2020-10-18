--[3] 해당 글을 세부적으로 읽어오는 저장 프로시저: ViewNote, NotesView
Create Procedure dbo.ViewNote
    @Id Int
As
    -- 조회수 카운트 1증가
    Update Notes Set ReadCount = ReadCount + 1 Where Id = @Id
    
    -- 모든 항목 조회
    Select * From Notes Where Id = @Id
Go
