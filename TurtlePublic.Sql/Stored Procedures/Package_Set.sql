CREATE PROCEDURE [dbo].[Package_Set]
    @id INT,
    @name NVARCHAR(250),
    @ownerId INT -- Used only on INSERT
AS

    UPDATE Package
    SET Name = @name
    WHERE Id = @id;

    IF @@ROWCOUNT = 0
    BEGIN
        INSERT INTO Package (
            Name,
            OwnerId)
        VALUES (
            @name,
            @ownerId);
        SELECT @id = SCOPE_IDENTITY();
    END

    EXEC Package_GetById @id = @id;

RETURN 0
