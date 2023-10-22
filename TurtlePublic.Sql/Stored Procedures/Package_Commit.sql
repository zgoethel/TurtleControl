CREATE PROCEDURE [dbo].[Package_Commit]
    @id INT = NULL,
    @path NVARCHAR(250),
    @description NVARCHAR(MAX),
    @_userId INT,
    @source NVARCHAR(MAX)
AS

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

    -- Mark package as having pending update, if
    -- source is associated with package
    UPDATE PackageSource
    SET IsDirty = 1
    WHERE PackageId = @id

RETURN 0
