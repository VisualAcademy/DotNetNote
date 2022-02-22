CREATE TABLE [dbo].[CabinetTypes] (
    [Id]             BIGINT         IDENTITY (1, 1) NOT NULL Primary Key,
    [Identification] NVARCHAR (MAX) NULL,
    [Show]           BIT            NULL,
    [Adjusted]       BIT            NOT NULL
);
