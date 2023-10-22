CREATE PROCEDURE [dbo].[Turtle_InstallPackage]
    @turtleId INT,
    @packageId INT
AS

    -- Does nothing if already installed
    IF NOT EXISTS (
        SELECT TOP (1) * FROM TurtlePackage
        WHERE TurtleId = @turtleId
            AND PackageId = @packageId)
    BEGIN
        INSERT INTO TurtlePackage (
            TurtleId,
            PackageId)
        VALUES (
            @turtleId,
            @packageId);
    END

RETURN 0
