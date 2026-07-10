CREATE TABLE dbo.EmployeeEmploymentAuditRecords
(
    ID BIGINT IDENTITY(1, 1) NOT NULL
        CONSTRAINT PK_EmployeeEmploymentAuditRecords PRIMARY KEY,

    AuditTrailRecordID BIGINT NOT NULL,
    EmployeeID BIGINT NOT NULL,
    EmployeeName NVARCHAR(300) NOT NULL,

    ChangedByUserID NVARCHAR(450) NULL,
    ChangedByUserName NVARCHAR(256) NOT NULL,
    ChangedByEmail NVARCHAR(256) NULL,
    ChangedByRole NVARCHAR(100) NULL,

    ChangedAt DATETIMEOFFSET NOT NULL,
    ChangeCount INT NOT NULL,

    TraceIdentifier NVARCHAR(100) NULL,
    RequestPath NVARCHAR(500) NULL,
    RemoteIpAddress NVARCHAR(64) NULL,

    Active BIT NOT NULL
        CONSTRAINT DF_EmployeeEmploymentAuditRecords_Active
        DEFAULT (1)
);
