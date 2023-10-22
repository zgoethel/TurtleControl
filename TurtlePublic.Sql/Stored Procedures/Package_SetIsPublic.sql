CREATE PROCEDURE [dbo].[Package_SetIsPublic]
    @id INT,
    @isPublic BIT
AS

    UPDATE Package
    SET
        IsPublic = @isPublic
    WHERE Id = @id;

    IF @@ROWCOUNT = 0
    BEGIN
        RAISERROR('No update was performed', 18, 1)
        RETURN 0
    END

    EXEC Package_GetById @id = @id

RETURN 0
