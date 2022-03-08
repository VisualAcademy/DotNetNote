CREATE TABLE [dbo].[Sublocation] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Sublocation] NVARCHAR (MAX) NULL,
    [Active]      BIT            NULL,
    [Location]    NVARCHAR (MAX) NULL,
    [Property]    NVARCHAR (MAX) NULL,
    [LocationId]  INT            NULL,
    CONSTRAINT [PK_Sublocation] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Sublocation_Location_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [dbo].[Location] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Sublocation_LocationId]
    ON [dbo].[Sublocation]([LocationId] ASC);

