CREATE TABLE [dbo].[TurtlePackage]
(
    [TurtleId] INT NOT NULL, 
    [PackageId] INT NOT NULL, 
    [Added] DATETIME NOT NULL DEFAULT GETDATE(), 
    [Published] DATETIME NULL, 
    CONSTRAINT [FK_TurtlePackage_PackageId] FOREIGN KEY ([PackageId]) REFERENCES [Package]([Id]), 
    CONSTRAINT [FK_TurtlePackage_TurtleId] FOREIGN KEY ([TurtleId]) REFERENCES [Turtle]([Id]), 
    CONSTRAINT [PK_TurtlePackage] PRIMARY KEY ([TurtleId], [PackageId])
)

GO
