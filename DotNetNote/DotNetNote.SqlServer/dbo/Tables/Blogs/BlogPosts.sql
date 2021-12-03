CREATE VIEW [dbo].[BlogPosts]
AS 
    Select 
        b.Name, b.BloggerName, 
        p.Title, p.Content, p.Created
    From  
        Posts p Join Blogs b 
            On p.BlogId = b.BlogId 
Go
