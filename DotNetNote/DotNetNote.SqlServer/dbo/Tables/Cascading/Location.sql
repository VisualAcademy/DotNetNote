CREATE TABLE [dbo].[Location] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [Name]       NVARCHAR (MAX) NULL,
    [Active]     BIT            NULL,
    [Property]   NVARCHAR (MAX) NULL,
    [PropertyId] INT            NULL,
    CONSTRAINT [PK_Location] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Location_Property_PropertyId] FOREIGN KEY ([PropertyId]) REFERENCES [dbo].[Property] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Location_PropertyId]
    ON [dbo].[Location]([PropertyId] ASC);

