CREATE TABLE AppLogs (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Message NVARCHAR(MAX),
    MessageTemplate NVARCHAR(MAX),
    Level NVARCHAR(128),
    TimeStamp DATETIMEOFFSET,
    Exception NVARCHAR(MAX),
    Properties NVARCHAR(MAX)
);