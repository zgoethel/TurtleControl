CREATE PROCEDURE [dbo].[Account_ResetPassword]
    @resetToken VARCHAR(100),
    @passwordScheme VARCHAR(20),
    @passwordHash VARCHAR(100),
    @passwordSalt VARCHAR(100)
AS

    DECLARE @id INT = (
        SELECT TOP (1) Id FROM Account WHERE ResetToken = @resetToken);

    UPDATE Account
    SET PasswordScheme = @passwordScheme,
        PasswordHash = @passwordHash,
        PasswordSalt = @passwordSalt,
        PasswordSet = GETDATE(),
        ResetToken = NULL
    WHERE Id = @id;

    EXEC Account_GetById @id;

RETURN 0
