CREATE TABLE [dbo].[Clients] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Type]        NCHAR (7)      NOT NULL,
    [FullName]    NVARCHAR (80)  NOT NULL,
    [MainAccount] MONEY          NOT NULL,
    [Address]     NVARCHAR (100) NOT NULL,
    [BankAccount] MONEY          NOT NULL,
    [Reliability] BIT            NOT NULL,
    [Credit]      MONEY          NOT NULL,
    [PhoneNumber] NCHAR (14)     NOT NULL,
    [Current]     TINYINT        NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

