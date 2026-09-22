CREATE TABLE [dbo].[InventoryTransactions]
(
    [Id] BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_InventoryTransactions] PRIMARY KEY,
    [InventoryStockId] INT NOT NULL,
    [RelatedInventoryStockId] INT NULL,
    [TransactionType] NVARCHAR(50) NOT NULL,
    [Quantity] DECIMAL(18,4) NOT NULL,
    [FromWarehouseLocationId] INT NULL,
    [ToWarehouseLocationId] INT NULL,
    [QuantityBefore] DECIMAL(18,4) NULL,
    [QuantityAfter] DECIMAL(18,4) NULL,
    [UnitCost] DECIMAL(19,4) NULL,
    [ReferenceType] NVARCHAR(100) NULL,
    [ReferenceId] NVARCHAR(255) NULL,
    [Notes] NVARCHAR(2000) NULL,
    [TransactionDate] DATETIME2(0) NOT NULL CONSTRAINT [DF_InventoryTransactions_TransactionDate] DEFAULT(SYSDATETIME()),
    [CreatedBy] NVARCHAR(255) NULL,
    [Created] DATETIME2(0) NULL CONSTRAINT [DF_InventoryTransactions_Created] DEFAULT(SYSDATETIME()),
    CONSTRAINT [FK_InventoryTransactions_InventoryStocks] FOREIGN KEY ([InventoryStockId]) REFERENCES [dbo].[InventoryStocks]([Id])
);
GO
CREATE INDEX [IX_InventoryTransactions_Stock_Date] ON [dbo].[InventoryTransactions]([InventoryStockId], [TransactionDate] DESC, [Id] DESC);
GO
CREATE INDEX [IX_InventoryTransactions_Type_Date] ON [dbo].[InventoryTransactions]([TransactionType], [TransactionDate] DESC, [Id] DESC);
GO
CREATE INDEX [IX_InventoryTransactions_Reference] ON [dbo].[InventoryTransactions]([ReferenceType], [ReferenceId]);
GO
