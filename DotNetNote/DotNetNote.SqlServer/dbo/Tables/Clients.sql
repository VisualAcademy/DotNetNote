CREATE TABLE [dbo].[Clients]
(
    [Id]               BIGINT IDENTITY(1,1) NOT NULL,
    [TenantId]         NVARCHAR(64)  NOT NULL,
    [OrganizationName] NVARCHAR(200) NOT NULL,
    [BillingEmail]     NVARCHAR(200) NOT NULL,
    [Domain]           NVARCHAR(200) NULL,
    [Type]             INT NOT NULL,

    CONSTRAINT [PK_Clients] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO
