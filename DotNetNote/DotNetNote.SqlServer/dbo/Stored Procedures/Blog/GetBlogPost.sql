CREATE PROCEDURE [dbo].[GetBlogPost]
AS
	Begin Transaction -- 의미 없지만, 학습상...
		Select 
			b.Name, b.BloggerName, 
			p.Title, p.Content, p.Created
		From  
			Posts p Join Blogs b 
				On p.BlogId = b.BlogId 
		Order By p.PostId Desc
		
		If @@ERROR > 0 
			RollBack Transaction 
	Commit Transaction
GO
