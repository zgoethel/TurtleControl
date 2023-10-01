CREATE PROCEDURE [dbo].[Turtle_List]
    @page INT,
    @count INT,
    @_userId INT
AS

    SELECT t.Id,
        CCType,
        CCNum,
        CohortId,
        RootPath,
        OwnerId,
        Paired,
        TRIM(a.FirstName + ' ' + a.LastName) 'OwnerName',
        le.LastUpdated 'LastEvent',
        le.Type 'LastEventType',
        IsPublic
    FROM Turtle t
    INNER JOIN Account a on a.Id = t.OwnerId
    OUTER APPLY (
        SELECT TOP (1)
            e.LastUpdated,
            e.Type
        FROM EventNetMaterial e
        WHERE e.CCNum = t.CCNum
        ORDER BY e.Id DESC) le
    WHERE @_userId = 0 OR OwnerId = @_userId
    ORDER BY CCType, CCNum, t.Id

    OFFSET (@page * @count) ROWS
    FETCH NEXT @count ROWS ONLY

RETURN 0
