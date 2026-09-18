CREATE TABLE [dbo].[InventoryCategories]
(
    [Id] INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_InventoryCategories] PRIMARY KEY,
    [Code] NVARCHAR(50) NOT NULL,
    [Name] NVARCHAR(150) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [SortOrder] INT NOT NULL CONSTRAINT [DF_InventoryCategories_SortOrder] DEFAULT(0),
    [IsActive] BIT NOT NULL CONSTRAINT [DF_InventoryCategories_IsActive] DEFAULT(1),
    [CreatedBy] NVARCHAR(255) NULL,
    [Created] DATETIME2(0) NULL CONSTRAINT [DF_InventoryCategories_Created] DEFAULT(SYSDATETIME()),
    [ModifiedBy] NVARCHAR(255) NULL,
    [Modified] DATETIME2(0) NULL
);
GO
CREATE UNIQUE INDEX [UX_InventoryCategories_Code] ON [dbo].[InventoryCategories]([Code]);
GO
