CREATE TABLE [dbo].[SourceVersion]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Path] NVARCHAR(250) NOT NULL, 
    [Committed] DATETIME NOT NULL DEFAULT GETDATE(), 
    [Description] NVARCHAR(MAX) NOT NULL DEFAULT (''), 
    [CommittedBy] INT NOT NULL, 
    [Source] NVARCHAR(MAX) NOT NULL DEFAULT ('')
)

GO

CREATE INDEX [IX_SourceVersion_Path] ON [dbo].[SourceVersion] ([Path])
