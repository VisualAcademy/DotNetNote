CREATE PROCEDURE [dbo].[GetTablesNoteByIdWithOutput]
	@Id int = 0,
	@Note NVarChar(Max) Output
AS
	Select @Note = Note From Tables Where Id = @Id
Go
