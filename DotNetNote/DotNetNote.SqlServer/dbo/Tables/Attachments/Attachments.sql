CREATE TABLE [dbo].[Attachments] (
    ID bigint IDENTITY(1,1) PRIMARY KEY,
    DateCreated datetimeoffset(7) NOT NULL,
    FileName nvarchar(max) NULL,
    EmployeeID bigint NULL
);
