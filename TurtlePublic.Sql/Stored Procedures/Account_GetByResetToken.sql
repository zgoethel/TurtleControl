CREATE PROCEDURE [dbo].[Account_GetByResetToken]
    @resetToken VARCHAR(100)
AS

    DECLARE @id INT = (
        SELECT TOP (1) Id FROM Account WHERE ResetToken = @resetToken);
    
    EXEC Account_GetWithReset @id;

RETURN 0
