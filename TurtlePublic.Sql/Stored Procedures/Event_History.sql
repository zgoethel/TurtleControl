CREATE PROCEDURE [dbo].[Event_History]
	--@RangeStart DATETIME = NULL,
	--@RangeEnd DATETIME = NULL
AS
BEGIN
	SET NOCOUNT ON;

	--SELECT
        --@RangeStart = ISNULL(@RangeStart, DATEADD(DAY, -1, GETDATE())),
		--@RangeEnd = ISNULL(@RangeEnd, DATEADD(DAY, 1, @RangeStart))
	
    SELECT TOP (100) *
	FROM EventNetMaterial enm
	--WHERE enm.Timestamp BETWEEN @RangeStart AND @RangeEnd
	ORDER BY enm.Id DESC
	FOR JSON PATH
END