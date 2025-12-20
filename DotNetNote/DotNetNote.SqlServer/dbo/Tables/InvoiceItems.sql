CREATE TABLE [dbo].[InvoiceItems]
(
    [Id]          BIGINT IDENTITY(1,1) NOT NULL,
    [InvoiceId]   BIGINT NOT NULL,
    [Description] NVARCHAR(200) NOT NULL,
    [Quantity]    DECIMAL(18,2) NOT NULL,
    [UnitPrice]   DECIMAL(18,2) NOT NULL,

    CONSTRAINT [PK_InvoiceItems] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO