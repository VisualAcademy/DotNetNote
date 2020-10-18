CREATE PROCEDURE [dbo].[GetTablesWithPaging]
	@PageIndex int = 0,		-- @PageIndex : 페이지 인덱스 : 0, 1, 2, ...
	@PageSize int = 10		-- @PageSize : 한 페이지에 표시할 레코드 수
AS
	Select Id, Note 
	From 
		(
			Select 
				ROW_NUMBER() Over (Order By Id Desc) As RowNumbers, Id, Note 
			From Tables
		)
		As TempRowTables
	Where 
		RowNumbers 
			Between 
				(@PageIndex * @PageSize + 1) 
			And 
				(@PageIndex + 1) * @PageSize
Go
