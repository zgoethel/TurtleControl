CREATE PROCEDURE [dbo].[Account_GetWithReset]
    @id INT
AS

    SELECT Id,
        Email,
        FirstName,
        LastName,
        ResetToken,
        ResetIssued
    FROM Account
    WHERE Id = @id

RETURN 0
