CREATE PROCEDURE [dbo].[Package_GetSourceVersionById]
    @id INT
AS

    SELECT Id,
        Path,
        Committed,
        Description,
        Committed,
        CommittedBy,
        Source
    FROM SourceVersion
    WHERE Id = @id

RETURN 0
