CREATE PROCEDURE [dbo].[Turtle_GetById]
    @id INT
AS

    SELECT Id,
        CCType,
        CCNum,
        CohortId,
        RootPath,
        OwnerId,
        Paired
    FROM Turtle
    WHERE Id = @id

RETURN 0
