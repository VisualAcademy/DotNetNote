CREATE TABLE [dbo].[Instructions]
(
    [Id]        BIGINT         IDENTITY (1, 1) NOT NULL PRIMARY KEY,
    [Active]    BIT            DEFAULT ((1)) NULL,
    [CreatedAt] DATETIMEOFFSET NULL DEFAULT SYSDATETIMEOFFSET(),
    [CreatedBy] NVARCHAR (255) NULL,
    [Name]      NVARCHAR (MAX) NULL,
    [Content]   NVARCHAR (MAX) NULL
);
