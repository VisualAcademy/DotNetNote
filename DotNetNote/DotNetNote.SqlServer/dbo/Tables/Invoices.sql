CREATE TABLE [dbo].[Invoices]
(
    [Id]           BIGINT IDENTITY(1,1) NOT NULL,

    -- BillingDbContext ̸  ʾ EF ⺻ NVARCHAR(MAX) ˴ϴ.
    [TenantId]     NVARCHAR(MAX) NOT NULL,

    [ClientId]     BIGINT NULL,

    [TenantName]   NVARCHAR(128) NOT NULL,
    [TenantKey]    NVARCHAR(128) NOT NULL,
    [Email]        NVARCHAR(256) NOT NULL,
    [EmailNormalized] NVARCHAR(256) NOT NULL,
    [FirstName]    NVARCHAR(100) NULL,
    [MiddleName]   NVARCHAR(100) NULL,
    [LastName]     NVARCHAR(100) NULL,
    [ClientName]   NVARCHAR(200) NULL,
    [ClientType]   NVARCHAR(64)  NULL,
    [InvoiceNumber] NVARCHAR(32) NULL,

    [IssueDateUtc] DATETIME2 NOT NULL,
    [DueDateUtc]   DATETIME2 NULL,

    [Currency]     NVARCHAR(8) NOT NULL,

    [ApplyTax]     BIT NOT NULL,
    [TaxRate]      DECIMAL(18,2) NOT NULL,
    [Subtotal]     DECIMAL(18,2) NOT NULL,
    [Tax]          DECIMAL(18,2) NOT NULL,
    [Total]        DECIMAL(18,2) NOT NULL,

    [Status]       INT NOT NULL,

    [PdfPath]      NVARCHAR(MAX) NULL,

    [CreatedUtc]   DATETIME2 NOT NULL,
    [UpdatedUtc]   DATETIME2 NOT NULL,
    [EmailSentUtc] DATETIME2 NULL,

    [IsDeleted]    BIT NOT NULL,
    [DeletedUtc]   DATETIME2 NULL,

    CONSTRAINT [PK_Invoices] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO
