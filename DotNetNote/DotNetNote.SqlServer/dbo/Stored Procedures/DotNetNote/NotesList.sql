-- 전체 데이터 조회(게시판 리스트)
CREATE PROCEDURE [dbo].[AnswersList]
    @PageNumber Int = 1,
    @PageSize Int = 10
AS
    Select 
        [Id], 
        [Name], 
        [PostDate], 
        [PostIp], 
        [Title], 
        [Category], 
        [ReadCount], 
        [FileName], 
        [FileSize], 
        [DownCount], 
        [CommentCount], 
        [Step]
    From Notes
    Order By Ref Desc, RefOrder Asc
    Offset ((@PageNumber - 1) * @PageSize) Rows Fetch Next @PageSize Rows Only;
Go
