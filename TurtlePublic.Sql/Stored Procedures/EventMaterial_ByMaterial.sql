CREATE PROCEDURE [dbo].[EventMaterial_ByMaterial]
	@RangeStart DATETIME = NULL,
	@RangeEnd DATETIME = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
        @RangeStart = ISNULL(@RangeStart, DATEADD(DAY, -1, GETDATE())),
		@RangeEnd = ISNULL(@RangeEnd, DATEADD(DAY, 1, @RangeStart))
	
    SELECT
        em.Material,
		ISNULL(SUM(em.NetAmount), 0) 'NetAmount'
	FROM EventMaterial em
	INNER JOIN Event e ON e.Id = em.EventId
	WHERE e.Timestamp BETWEEN @RangeStart AND @RangeEnd
	GROUP BY em.Material
	ORDER BY ISNULL(SUM(em.NetAmount), 0) DESC, em.Material
	FOR JSON PATH
END
GO