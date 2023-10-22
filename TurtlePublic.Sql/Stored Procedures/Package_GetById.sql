CREATE PROCEDURE [dbo].[Package_GetById]
    @id INT
AS

    SELECT Id,
        Name,
        OwnerId,
        Created,
        IsPublic
    FROM Package
    WHERE Id = @id;

RETURN 0
