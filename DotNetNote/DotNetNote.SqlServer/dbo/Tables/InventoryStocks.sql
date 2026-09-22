CREATE TABLE [dbo].[InventoryStocks]
(
    [Id] INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_InventoryStocks] PRIMARY KEY,
    [InventoryItemId] INT NOT NULL,
    [WarehouseLocationId] INT NULL,
    [SerialNumber] NVARCHAR(255) NULL,
    [LotNumber] NVARCHAR(255) NULL,
    [AssetTag] NVARCHAR(255) NULL,
    [Quantity] DECIMAL(18,4) NOT NULL,
    [Status] NVARCHAR(50) NOT NULL CONSTRAINT [DF_InventoryStocks_Status] DEFAULT(N'Available'),
    [ReceivedDate] DATETIME2(0) NULL,
    [UnitCost] DECIMAL(19,4) NULL,
    [Currency] NVARCHAR(10) NULL CONSTRAINT [DF_InventoryStocks_Currency] DEFAULT(N'USD'),
    [SupplierName] NVARCHAR(255) NULL,
    [PurchaseOrderNumber] NVARCHAR(255) NULL,
    [InvoiceNumber] NVARCHAR(255) NULL,
    [ExternalReference] NVARCHAR(255) NULL,
    [Notes] NVARCHAR(2000) NULL,
    [IsActive] BIT NOT NULL CONSTRAINT [DF_InventoryStocks_IsActive] DEFAULT(1),
    [CreatedBy] NVARCHAR(255) NULL,
    [Created] DATETIME2(0) NULL CONSTRAINT [DF_InventoryStocks_Created] DEFAULT(SYSDATETIME()),
    [ModifiedBy] NVARCHAR(255) NULL,
    [Modified] DATETIME2(0) NULL,
    CONSTRAINT [FK_InventoryStocks_InventoryItems] FOREIGN KEY ([InventoryItemId]) REFERENCES [dbo].[InventoryItems]([Id]),
    CONSTRAINT [FK_InventoryStocks_WarehouseLocations] FOREIGN KEY ([WarehouseLocationId]) REFERENCES [dbo].[WarehouseLocations]([Id])
);
GO
CREATE INDEX [IX_InventoryStocks_Item_Serial] ON [dbo].[InventoryStocks]([InventoryItemId], [SerialNumber]);
GO
CREATE INDEX [IX_InventoryStocks_Location_Status] ON [dbo].[InventoryStocks]([WarehouseLocationId], [Status]);
GO
CREATE INDEX [IX_InventoryStocks_AssetTag] ON [dbo].[InventoryStocks]([AssetTag]);
GO
CREATE INDEX [IX_InventoryStocks_Lot] ON [dbo].[InventoryStocks]([LotNumber]);
GO
