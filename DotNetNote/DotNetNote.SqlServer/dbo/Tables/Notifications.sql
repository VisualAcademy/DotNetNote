-- =============================================
-- Table: dbo.Notifications
-- Description:
--   Centralized notification table used across
--   all application modules and tenant projects.
--   This table stores system messages, alerts,
--   and user-targeted notifications.
-- =============================================

CREATE TABLE [dbo].[Notifications]
(
    -- Primary identifier
    [ID] BIGINT IDENTITY(1,1) NOT NULL,

    -- UTC timestamp when the notification was created
    [DateCreated] DATETIMEOFFSET(7) NOT NULL
        CONSTRAINT [DF_Notifications_DateCreated]
        DEFAULT (SYSUTCDATETIME()),

    -- Indicates whether the notification is active
    [Active] BIT NOT NULL
        CONSTRAINT [DF_Notifications_Active]
        DEFAULT ((1)),

    -- Optional timestamp for auditing or tracking creation workflows
    [CreatedAt] DATETIMEOFFSET(7) NULL
        CONSTRAINT [DF_Notifications_CreatedAt]
        DEFAULT (SYSUTCDATETIME()),

    -- Optional identifier of the creator (username, system, service name, etc.)
    [CreatedBy] NVARCHAR(70) NULL,

    -- Notification message body
    [Message] NVARCHAR(MAX) NULL,

    -- Logical classification of notification (Info, Warning, Error, System, etc.)
    [Type] NVARCHAR(MAX) NULL,

    -- Optional navigation URL related to the notification
    [Url] NVARCHAR(MAX) NULL,

    -- Optional reference to an employee entity (relationship handled programmatically)
    [EmployeeId] BIGINT NULL,

    -- Optional reference to a vendor entity (relationship handled programmatically)
    [VendorId] BIGINT NULL,

    -- Application-specific categorization value
    [ApplicantType] INT NOT NULL
        CONSTRAINT [DF_Notifications_ApplicantType]
        DEFAULT ((0)),

    CONSTRAINT [PK_Notifications]
        PRIMARY KEY CLUSTERED ([ID] ASC)
);
GO


-- =============================================
-- Indexes
-- =============================================

-- Index to optimize queries filtering by EmployeeId
CREATE NONCLUSTERED INDEX [IX_Notifications_EmployeeId]
    ON [dbo].[Notifications]([EmployeeId] ASC);
GO

-- Index to optimize queries filtering by VendorId
CREATE NONCLUSTERED INDEX [IX_Notifications_VendorId]
    ON [dbo].[Notifications]([VendorId] ASC);
GO


-- =============================================
-- Optional Foreign Key Constraints (Currently Disabled)
-- -----------------------------------------------------
-- The following constraints are intentionally commented out.
-- Relationships are managed at the application layer.
-- Uncomment only if strict database-level enforcement is required.
-- =============================================

/*
ALTER TABLE [dbo].[Notifications] WITH CHECK
ADD CONSTRAINT [FK_Notifications_Employees_EmployeeId]
FOREIGN KEY ([EmployeeId])
REFERENCES [dbo].[Employees] ([ID]);
GO

ALTER TABLE [dbo].[Notifications]
CHECK CONSTRAINT [FK_Notifications_Employees_EmployeeId];
GO
*/


/*
ALTER TABLE [dbo].[Notifications] WITH CHECK
ADD CONSTRAINT [FK_Notifications_Vendors_VendorId]
FOREIGN KEY ([VendorId])
REFERENCES [dbo].[Vendors] ([ID]);
GO

ALTER TABLE [dbo].[Notifications]
CHECK CONSTRAINT [FK_Notifications_Vendors_VendorId];
GO
*/
