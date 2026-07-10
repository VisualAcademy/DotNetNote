CREATE TABLE dbo.EmployeeEmploymentAuditChanges
(
    ID BIGINT IDENTITY(1, 1) NOT NULL
        CONSTRAINT PK_EmployeeEmploymentAuditChanges PRIMARY KEY,

    AuditRecordID BIGINT NOT NULL,

    PropertyName NVARCHAR(100) NOT NULL,
    DisplayName NVARCHAR(150) NOT NULL,

    OldValue NVARCHAR(MAX) NULL,
    NewValue NVARCHAR(MAX) NULL,

    OldDisplayValue NVARCHAR(500) NULL,
    NewDisplayValue NVARCHAR(500) NULL,

    DataType NVARCHAR(50) NOT NULL,
    SortOrder INT NOT NULL,
    ChangedAt DATETIMEOFFSET NOT NULL,

    Active BIT NOT NULL
        CONSTRAINT DF_EmployeeEmploymentAuditChanges_Active
        DEFAULT (1),

    CONSTRAINT FK_EmployeeEmploymentAuditChanges_Record
        FOREIGN KEY (AuditRecordID)
        REFERENCES dbo.EmployeeEmploymentAuditRecords(ID)
        ON DELETE CASCADE
);
