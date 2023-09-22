CREATE PROCEDURE [dbo].[Account_List]
    @page INT,
    @count INT
AS

    SELECT Id,
        Email,
        FirstName,
        LastName
    FROM Account
    ORDER BY LastName, FirstName, Id

    OFFSET (@page * @count) ROWS
    FETCH NEXT @count ROWS ONLY

RETURN 0
