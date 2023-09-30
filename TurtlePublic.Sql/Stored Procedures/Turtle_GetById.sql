CREATE PROCEDURE [dbo].[Turtle_GetById]
    @id INT
AS

    SELECT t.Id,
        CCType,
        CCNum,
        CohortId,
        RootPath,
        OwnerId,
        Paired,
        TRIM(a.FirstName + ' ' + a.LastName) 'OwnerName'
    FROM Turtle t
    INNER JOIN Account a ON a.Id = t.OwnerId
    WHERE t.Id = @id

RETURN 0
