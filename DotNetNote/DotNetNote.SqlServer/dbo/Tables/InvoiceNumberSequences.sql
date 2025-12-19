CREATE TABLE [dbo].[InvoiceNumberSequences]
(
    -- EF Core convention상 string PK 길이 제한이 없으면 보통 NVARCHAR(450)로 잡힙니다.
    [TenantId]   NVARCHAR(450) NOT NULL,
    [NextValue]  BIGINT NOT NULL,

    CONSTRAINT [PK_InvoiceNumberSequences] PRIMARY KEY CLUSTERED ([TenantId] ASC)
);
GO