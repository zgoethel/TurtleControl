CREATE PROCEDURE Event_Cleanup
AS
BEGIN
	SET NOCOUNT ON;

    DECLARE @expiredIds TABLE (Id INT);

	INSERT INTO @expiredIds
	SELECT Id FROM Event
	WHERE Timestamp < DATEADD(DAY, -3, GETDATE());
	
	DELETE FROM EventMaterial
	WHERE EventId IN (SELECT ex.Id FROM @expiredIds ex)
	
	DELETE FROM Event
	WHERE Id IN (SELECT ex.Id FROM @expiredIds ex)
END
GO