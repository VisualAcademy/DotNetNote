CREATE TABLE [dbo].[Property] (
    [Id]     INT            IDENTITY (1, 1) NOT NULL,
    [Name]   NVARCHAR (MAX) NULL,
    [Active] BIT            NULL,
    CONSTRAINT [PK_Property] PRIMARY KEY CLUSTERED ([Id] ASC)
);

