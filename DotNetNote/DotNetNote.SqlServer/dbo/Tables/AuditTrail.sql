CREATE TABLE [dbo].[AuditTrail]
(
    [ID]                BIGINT             IDENTITY(1,1) NOT NULL,
    [Active]            BIT                NOT NULL CONSTRAINT [DF_AuditTrail_Active] DEFAULT (1),

    [CreatedAt]         DATETIMEOFFSET(7)  NOT NULL,
    [CreatedBy]         NVARCHAR(70)       NULL,

    [EntityID]          BIGINT             NOT NULL,
    [EntityName]        NVARCHAR(128)      NULL,
    [Operation]         NVARCHAR(70)       NULL,

    -- 기존 솔루션들 간 컬럼 차이 통합(모두 NULL 허용)
    [EmployeeID]        BIGINT             NULL,
    [InvestigationID]   BIGINT             NULL,
    [Note]              NVARCHAR(128)      NULL,

    -- (Vendor 쪽에만 있던 컬럼들)
    [Type]              BIT                NULL,
    [OwnerID]           NVARCHAR(50)       NULL,
    [OwnerName]         NVARCHAR(MAX)      NULL,
    [Investigation]     NVARCHAR(50)       NULL,

    -- 추가: 테넌트/프로젝트 추적용(요청)
    [TenantName]        NVARCHAR(128)      NULL,
    [ProjectName]       NVARCHAR(128)      NULL,
    [EnvironmentName]   NVARCHAR(50)       NULL,

    CONSTRAINT [PK_AuditTrail] PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO

/* (선택) 조회 성능을 위한 인덱스 - 기존 앱 영향 없이 "추가"만 하므로 비교적 안전
CREATE NONCLUSTERED INDEX [IX_AuditTrail_CreatedAt]
ON [dbo].[AuditTrail]([CreatedAt] DESC);

CREATE NONCLUSTERED INDEX [IX_AuditTrail_Entity]
ON [dbo].[AuditTrail]([EntityName], [EntityID]);

CREATE NONCLUSTERED INDEX [IX_AuditTrail_Tenant_Project]
ON [dbo].[AuditTrail]([TenantName], [ProjectName], [CreatedAt] DESC);
*/
