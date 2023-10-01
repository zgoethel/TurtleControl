CREATE TABLE [dbo].[Turtle]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CCType] VARCHAR(50) NOT NULL, 
    [CCNum] INT NOT NULL, 
    [CohortId] INT NULL, 
    [RootPath] VARCHAR(200) NOT NULL, 
    [OwnerId] INT NOT NULL, 
    [Paired] DATETIME NOT NULL DEFAULT GETDATE(), 
    [IsPublic] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [AK_Turtle_CCNum] UNIQUE ([CCNum]) 
)

GO

CREATE INDEX [IX_Turtle_CohortId] ON [dbo].[Turtle] ([CohortId])
GO

CREATE INDEX [IX_Turtle_OwnerId] ON [dbo].[Turtle] ([OwnerId])
GO