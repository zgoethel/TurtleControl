CREATE PROCEDURE [dbo].[Package_Commit]
    @id INT = NULL,
    @path NVARCHAR(250),
    @description NVARCHAR(MAX),
    @_userId INT,
    @source NVARCHAR(MAX)
AS

    -- Mark package as having pending update, if
    -- source is associated with package
    UPDATE PackageSource
    SET IsDirty = 1
    WHERE PackageId = @id;

    INSERT INTO SourceVersion (
        Path,
        Description,
        CommittedBy,
        Source)
    VALUES (
        @path,
        @description,
        @_userId,
        @source);

    DECLARE @_id INT = SCOPE_IDENTITY();
    EXEC Package_GetSourceVersionById @id = @_id;

RETURN 0
