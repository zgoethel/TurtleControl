CREATE PROCEDURE [dbo].[Turtle_List]
    @page INT,
    @count INT,
    @_userId INT
AS

    SELECT Id,
        CCType,
        CCNum,
        CohortId,
        RootPath,
        OwnerId,
        Paired
    FROM Turtle
    WHERE @_userId = 0 OR OwnerId = @_userId
    ORDER BY CCType, CCNum, Id

    OFFSET (@page * @count) ROWS
    FETCH NEXT @count ROWS ONLY

RETURN 0
