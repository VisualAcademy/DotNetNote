CREATE TABLE [dbo].[InventoryItems]
(
    [Id] INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_InventoryItems] PRIMARY KEY,
    [InventoryCategoryId] INT NOT NULL,
    [ItemCode] NVARCHAR(100) NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(1000) NULL,
    [Manufacturer] NVARCHAR(255) NULL,
    [Model] NVARCHAR(255) NULL,
    [PartNumber] NVARCHAR(255) NULL,
    [TrackingType] NVARCHAR(30) NOT NULL CONSTRAINT [DF_InventoryItems_TrackingType] DEFAULT(N'Quantity'),
    [UnitOfMeasure] NVARCHAR(50) NOT NULL CONSTRAINT [DF_InventoryItems_UnitOfMeasure] DEFAULT(N'Each'),
    [IsActive] BIT NOT NULL CONSTRAINT [DF_InventoryItems_IsActive] DEFAULT(1),
    [CreatedBy] NVARCHAR(255) NULL,
    [Created] DATETIME2(0) NULL CONSTRAINT [DF_InventoryItems_Created] DEFAULT(SYSDATETIME()),
    [ModifiedBy] NVARCHAR(255) NULL,
    [Modified] DATETIME2(0) NULL,
    CONSTRAINT [FK_InventoryItems_InventoryCategories] FOREIGN KEY ([InventoryCategoryId]) REFERENCES [dbo].[InventoryCategories]([Id])
);
GO
CREATE UNIQUE INDEX [UX_InventoryItems_ItemCode] ON [dbo].[InventoryItems]([ItemCode]);
GO
CREATE INDEX [IX_InventoryItems_Category] ON [dbo].[InventoryItems]([InventoryCategoryId], [IsActive], [Name]);
GO
