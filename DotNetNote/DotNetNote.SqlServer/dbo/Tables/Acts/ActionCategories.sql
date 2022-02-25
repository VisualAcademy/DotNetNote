CREATE TABLE [dbo].[ActionCategories] (
    [ID]         BIGINT         IDENTITY (1, 1) NOT NULL,
    [gbAdjusted] BIT            CONSTRAINT [DF_ActionCategories_gbAdjusted] DEFAULT ((0)) NOT NULL,
    [Category]   NVARCHAR (255) NULL,
    [Active]     BIT            NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

