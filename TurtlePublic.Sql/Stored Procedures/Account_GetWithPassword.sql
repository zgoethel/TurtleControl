CREATE PROCEDURE [dbo].[Account_GetWithPassword]
    @email NVARCHAR(50)
AS

    SELECT Id,
        Email,
        FirstName,
        LastName,
        PasswordScheme,
        PasswordHash,
        PasswordSalt,
        PasswordSet
    FROM Account
    WHERE Email = @email

RETURN 0
