CREATE PROCEDURE [dbo].[Account_GetById]
    @id INT
AS

    SELECT Id,
        Email,
        FirstName,
        LastName
    FROM Account
    WHERE Id = @id

RETURN 0
