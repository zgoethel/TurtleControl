CREATE TABLE [dbo].[EventMaterial] (
    [EventId]      INT          NOT NULL,
    [Material]     VARCHAR (50) NOT NULL,
    [NetAmount]    INT          CONSTRAINT [DF_EventMaterial_NetAmount] DEFAULT ((0)) NOT NULL,
    [Transactions] INT          CONSTRAINT [DF_EventMaterial_Transactions] DEFAULT ((0)) NOT NULL,
    [LastUpdated]  DATETIME     CONSTRAINT [DF_EventMaterial_LastUpdated] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_EventMaterial] PRIMARY KEY CLUSTERED ([EventId] ASC, [Material] ASC),
    CONSTRAINT [FK_EventMaterial_Event] FOREIGN KEY ([EventId]) REFERENCES [dbo].[Event] ([Id])
);

