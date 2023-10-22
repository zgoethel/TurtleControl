CREATE TABLE [dbo].[PackageSource]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [PackageId] INT NOT NULL, 
    [Path] NVARCHAR(250) NULL, 
    [IsDirty] BIT NOT NULL DEFAULT 0, 
    [VersionId] INT NULL, 
    [Added] DATETIME NOT NULL DEFAULT GETDATE(), 
    CONSTRAINT [FK_PackageSource_PackageId] FOREIGN KEY ([PackageId]) REFERENCES [Package]([Id]), 
    CONSTRAINT [FK_PackageSource_VersionId] FOREIGN KEY ([VersionId]) REFERENCES [SourceVersion]([Id])
)

GO

CREATE INDEX [IX_PackageSource_PackageId] ON [dbo].[PackageSource] ([PackageId])

GO

CREATE INDEX [IX_PackageSource_Path] ON [dbo].[PackageSource] ([Path])

GO

CREATE INDEX [IX_PackageSource_VersionId] ON [dbo].[PackageSource] ([VersionId])
