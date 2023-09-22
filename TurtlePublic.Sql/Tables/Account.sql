CREATE TABLE [dbo].[Account]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Email] NVARCHAR(50) NOT NULL, 
    [FirstName] NVARCHAR(50) NOT NULL, 
    [LastName] NVARCHAR(50) NOT NULL, 
    [PasswordScheme] VARCHAR(20) NULL, 
    [PasswordHash] VARCHAR(100) NULL, 
    [PasswordSalt] VARCHAR(100) NULL, 
    [PasswordSet] DATETIME NULL, 
    [ResetToken] VARCHAR(100) NULL, 
    [ResetIssued] DATETIME NULL, 
    CONSTRAINT [AK_Account_Email] UNIQUE ([Email])
)

GO

CREATE INDEX [IX_Account_ResetToken] ON [dbo].[Account] ([ResetToken])
