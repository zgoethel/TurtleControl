CREATE TABLE [dbo].[Event] (
    [Id]        INT          IDENTITY (1, 1) NOT NULL,
    [Type]      VARCHAR (50) NOT NULL,
    [Timestamp] DATETIME     CONSTRAINT [DF_Event_Timestamp] DEFAULT (getdate()) NOT NULL,
    [CCType]    VARCHAR (50) NOT NULL,
    [CCNum]     INT          NOT NULL,
    CONSTRAINT [PK_Event] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Event_CCType_CCNum]
    ON [dbo].[Event]([CCType] ASC, [CCNum] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Event_Timestamp]
    ON [dbo].[Event]([Timestamp] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Event_Type]
    ON [dbo].[Event]([Type] ASC);

