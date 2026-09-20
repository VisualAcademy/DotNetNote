CREATE TABLE [dbo].[WarehouseLocations]
(
    [Id] INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_WarehouseLocations] PRIMARY KEY,
    [WarehouseId] INT NOT NULL,
    [ParentId] INT NULL,
    [Code] NVARCHAR(100) NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
    [LocationType] NVARCHAR(50) NULL,
    [Zone] NVARCHAR(100) NULL,
    [Aisle] NVARCHAR(100) NULL,
    [Rack] NVARCHAR(100) NULL,
    [Shelf] NVARCHAR(100) NULL,
    [Bin] NVARCHAR(100) NULL,
    [Description] NVARCHAR(500) NULL,
    [SortOrder] INT NOT NULL CONSTRAINT [DF_WarehouseLocations_SortOrder] DEFAULT(0),
    [IsActive] BIT NOT NULL CONSTRAINT [DF_WarehouseLocations_IsActive] DEFAULT(1),
    [CreatedBy] NVARCHAR(255) NULL,
    [Created] DATETIME2(0) NULL CONSTRAINT [DF_WarehouseLocations_Created] DEFAULT(SYSDATETIME()),
    [ModifiedBy] NVARCHAR(255) NULL,
    [Modified] DATETIME2(0) NULL,
    CONSTRAINT [FK_WarehouseLocations_Warehouses] FOREIGN KEY ([WarehouseId]) REFERENCES [dbo].[Warehouses]([Id]),
    CONSTRAINT [FK_WarehouseLocations_Parent] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[WarehouseLocations]([Id])
);
GO
CREATE UNIQUE INDEX [UX_WarehouseLocations_Warehouse_Code] ON [dbo].[WarehouseLocations]([WarehouseId], [Code]);
GO
CREATE INDEX [IX_WarehouseLocations_Parent] ON [dbo].[WarehouseLocations]([ParentId]);
GO
