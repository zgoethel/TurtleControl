CREATE PROCEDURE [dbo].[Account_SetResetToken]
    @email NVARCHAR(50),
    @resetToken VARCHAR(100)
AS

    DECLARE @id INT = (
        SELECT TOP (1) Id FROM Account WHERE Email = @email);

    UPDATE Account
    SET ResetToken = @resetToken,
        ResetIssued = GETDATE()
    WHERE Id = @id;

    EXEC Account_GetWithReset @id;

RETURN 0
