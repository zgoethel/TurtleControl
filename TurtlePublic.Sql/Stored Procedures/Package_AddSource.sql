CREATE PROCEDURE [dbo].[Package_AddSource]
    @id INT,
    @path NVARCHAR(250),
    @source NVARCHAR(MAX),
    @turtleId INT,
    @_userId INT
AS

    -- Does nothing if already installed
    EXEC Turtle_InstallPackage @turtleId = @turtleId,
        @packageId = @id;

    -- Remove existing associations
    DELETE FROM PackageSource
    WHERE Path = @path

    INSERT INTO PackageSource (
        PackageId,
        Path)
    VALUES (
        @id,
        @path);

    EXEC Package_Commit
        @path = @path,
        @description = 'Linked source to package',
        @_userId = @_userId,
        @source = source;

RETURN 0
