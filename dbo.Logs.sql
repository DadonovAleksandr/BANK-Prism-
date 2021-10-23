CREATE TABLE [dbo].[Logs] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Date]        DATETIME2 (2) NOT NULL,
    [MoneyAmount] MONEY         NOT NULL,
    [SenderID]    INT           NOT NULL,
    [RecipientID] INT           NOT NULL,
    [Successful]  BIT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

