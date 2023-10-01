CREATE PROCEDURE [dbo].[Event_Begin]
	@Type VARCHAR(50),
	@CCType VARCHAR(50),
	@CCNum INT
AS
BEGIN
	SET NOCOUNT ON;

    UPDATE Turtle
    SET CCType = @CCType
    WHERE CCNum = @CCNum

    INSERT INTO Event (
		Type,
		CCType,
		CCNum)
	VALUES (
		@Type,
		@CCType,
		@CCNum)

	SELECT SCOPE_IDENTITY() AS Id FOR JSON PATH
END
