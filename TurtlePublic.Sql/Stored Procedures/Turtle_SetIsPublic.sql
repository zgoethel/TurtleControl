CREATE PROCEDURE [dbo].[Turtle_SetIsPublic]
    @id INT,
    @isPublic BIT
AS

    UPDATE Turtle
    SET
        IsPublic = @isPublic
    WHERE Id = @id;

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('No update was performed', 18, 1)
        RETURN 0
    END

    EXEC Turtle_GetById @id = @id

RETURN 0
