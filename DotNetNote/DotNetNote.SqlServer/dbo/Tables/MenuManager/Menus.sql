CREATE TABLE [dbo].[Menus]
(
	[MenuId] INT Identity(1, 1) NOT NULL PRIMARY KEY,
	[MenuOrder] Int Not Null,

	[ParentId] Int Default(0),
	[MenuName] NVarChar(100) Not Null,
	[MenuPath] NVarChar(255) Null,
	[IsVisible] Bit Default(1) Not Null,

	[CommunityId] Int Default(1),			-- 커뮤니티별, TabId, PortalId

	[IsBoard] Bit Default(1) Not Null,		-- 기본값은 게시판
	[Target] NVarChar(20) Default('_self'),	-- _self, _blank
	[BoardAlias] NVarChar(50) Null			-- 게시판 별칭 추가 저장
)
Go

--SET IDENTITY_INSERT [dbo].[Menus] ON
--INSERT INTO [dbo].[Menus] 
--	([MenuId], [MenuOrder], [ParentId], [MenuName], [MenuPath], [IsVisible]) 
--VALUES (1, 1, 0, N'책(SQL)', N'/Home/Book', 1)
--INSERT INTO [dbo].[Menus] 
--	([MenuId], [MenuOrder], [ParentId], [MenuName], [MenuPath], [IsVisible]) 
--VALUES (2, 2, 0, N'강의(SQL)', N'/Home/Lecture', 1)
--INSERT INTO [dbo].[Menus] 
--	([MenuId], [MenuOrder], [ParentId], [MenuName], [MenuPath], [IsVisible]) 
--VALUES (3, 3, 1, N'좋은책(SQL)', NULL, 1)
--INSERT INTO [dbo].[Menus] 
--	([MenuId], [MenuOrder], [ParentId], [MenuName], [MenuPath], [IsVisible]) 
--VALUES (4, 4, 1, N'나쁜책(SQL)', NULL, 1)
--SET IDENTITY_INSERT [dbo].[Menus] OFF
