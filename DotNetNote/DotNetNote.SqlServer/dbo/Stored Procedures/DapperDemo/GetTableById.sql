CREATE PROCEDURE [dbo].[GetTableById]
	@Id int
AS
	Select [Id], [Note] From Tables Where Id = @Id
Go
