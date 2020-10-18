CREATE VIEW [dbo].[BoardViews]
AS 
	SELECT 
		[TID] As BoardId, 
		[BoardAlias], 
		[Title], 
		[Description], 
		[SysopUID] As SysopUsername, 
		[IsPublic], 
		[GroupName], 
		[GroupOrder], 
		[MailEnable], 
		[ShowList], 
		[BoardStyle], 
		[HeaderHtml], 
		[FooterHtml], 
		[MainShowList] 
	FROM [Boards]
Go
