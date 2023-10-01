CREATE PROCEDURE [dbo].[Turtle_Set]
    @id INT,
    @cohortId INT = NULL
AS

    UPDATE Turtle
    SET
        CohortId = @cohortId
    WHERE Id = @id;

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('No update was performed', 18, 1)
        RETURN 0
    END

    EXEC Turtle_GetById @id = @id

RETURN 0
