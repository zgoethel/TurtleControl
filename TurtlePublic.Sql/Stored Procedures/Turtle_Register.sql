CREATE PROCEDURE [dbo].[Turtle_Register]
    @ccType VARCHAR(50),
    @ccNum INT,
    @rootPath VARCHAR(200),
    @_userId INT
AS
BEGIN
	SET NOCOUNT ON;

    -- Validation error message; there is also a proper uniqueness
    -- constraint on the table column for integrity
    IF EXISTS (
        SELECT TOP (1) * FROM Turtle WHERE CCNum = @ccNum)
    BEGIN
        RAISERROR('Error: ComputerCraft identifier is already registered', 18, 1)
        RETURN
    END

    DECLARE @knownType VARCHAR(50) = (
        SELECT TOP (1) e.CCType FROM Event e
        WHERE e.CCNum = @ccNum
        ORDER BY e.Id DESC);

    INSERT INTO Turtle (
        CCType,
        CCNum,
        RootPath,
        OwnerId)
    VALUES (
        ISNULL(@knownType, @ccType),
        @ccNum,
        @rootPath,
        @_userId);

    DECLARE @id INT = (
        SELECT SCOPE_IDENTITY());

    EXEC Turtle_GetById @id;

END
GO
