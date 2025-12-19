CREATE TABLE [dbo].[InvoiceItemTemplates]
(
    [Id]              BIGINT IDENTITY(1,1) NOT NULL,

    -- SaaS Tenant scope
    -- NULL/'' = Global template (shared across all tenants)
    [TenantId]        NVARCHAR(450) NULL,
    [TenantKey]       NVARCHAR(128) NULL,
    [TenantName]      NVARCHAR(128) NULL,

    -- UI/Business
    [Name]            NVARCHAR(200) NOT NULL,   -- DropDown 표시용(짧은 이름)
    [Description]     NVARCHAR(200) NOT NULL,   -- InvoiceItems.Description로 복사될 기본값
    [DefaultQuantity] DECIMAL(18,2) NOT NULL CONSTRAINT [DF_InvoiceItemTemplates_DefaultQuantity] DEFAULT(1),
    [UnitPrice]       DECIMAL(18,2) NOT NULL,

    -- Optional (billing rules)
    [Currency]        NVARCHAR(8) NULL,         -- NULL이면 Invoice.Currency 사용
    [IsTaxable]       BIT NOT NULL CONSTRAINT [DF_InvoiceItemTemplates_IsTaxable] DEFAULT(0),

    -- 운영/관리
    [IsActive]        BIT NOT NULL CONSTRAINT [DF_InvoiceItemTemplates_IsActive] DEFAULT(1),
    [SortOrder]       INT NOT NULL CONSTRAINT [DF_InvoiceItemTemplates_SortOrder] DEFAULT(0),

    -- Soft delete (Settings에서 Delete 시 사용)
    [IsDeleted]       BIT NOT NULL CONSTRAINT [DF_InvoiceItemTemplates_IsDeleted] DEFAULT(0),
    [DeletedUtc]      DATETIME2 NULL,

    [CreatedUtc]      DATETIME2 NOT NULL CONSTRAINT [DF_InvoiceItemTemplates_CreatedUtc] DEFAULT(SYSUTCDATETIME()),
    [UpdatedUtc]      DATETIME2 NOT NULL CONSTRAINT [DF_InvoiceItemTemplates_UpdatedUtc] DEFAULT(SYSUTCDATETIME()),

    CONSTRAINT [PK_InvoiceItemTemplates] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- 공용/테넌트별 목록 조회가 잦으니 인덱스 추천
CREATE INDEX [IX_InvoiceItemTemplates_TenantId_IsActive_SortOrder]
ON [dbo].[InvoiceItemTemplates] ([TenantId], [IsActive], [SortOrder], [Name]);
GO

-- 테넌트 내에서 같은 이름 중복 방지(공용/전용 각각)
-- (TenantId NULL 공용도 하나의 그룹이 되도록 처리하려면 필터 인덱스 2개로 나누는 게 안전)
CREATE UNIQUE INDEX [UX_InvoiceItemTemplates_TenantId_Name]
ON [dbo].[InvoiceItemTemplates] ([TenantId], [Name]);
GO
