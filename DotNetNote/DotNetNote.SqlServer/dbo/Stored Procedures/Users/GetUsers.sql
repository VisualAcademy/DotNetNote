CREATE PROCEDURE [dbo].[GetUsers]
	@UID int = 0
AS
	Select [UID], [UserID], [Password], [Username] 
	From Users 
	Where UID = @UID
Go
