CREATE PROCEDURE [dbo].[Account_SignUp]
    @email NVARCHAR(50),
    @firstName NVARCHAR(50),
    @lastName NVARCHAR(50)
AS

    -- Validation error message; there is also a proper uniqueness
    -- constraint on the table column for integrity
    IF EXISTS (
        SELECT TOP (1) * FROM Account WHERE Email = @email)
    BEGIN
        RAISERROR('Email address is already in use', 18, 1)
    END

    INSERT INTO Account (
        Email,
        FirstName,
        LastName)
    VALUES (
        @email,
        @firstName,
        @lastName)

    DECLARE @id INT = (
        SELECT SCOPE_IDENTITY());

    EXEC Account_GetById @id;

RETURN 0
