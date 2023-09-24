CREATE PROCEDURE [dbo].[Event_Dashboard]
	@RangeStart DATETIME = NULL,
	@RangeEnd DATETIME = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
        @RangeStart = ISNULL(@RangeStart, DATEADD(DAY, -1, GETDATE())),
		@RangeEnd = ISNULL(@RangeEnd, DATEADD(DAY, 1, @RangeStart))
	
    SELECT

        (SELECT GETDATE()) 'Now',

		(SELECT ISNULL(COUNT(*), 0)
         FROM Event e
		 WHERE e.Type = 'HarvestTree'
			AND e.Timestamp BETWEEN @RangeStart AND @RangeEnd)
		 'HarvestedTrees',

		(SELECT ISNULL(SUM(em.NetAmount), 0)
         FROM EventMaterial em
		 INNER JOIN Event e ON e.Id = em.EventId
		 WHERE e.Type = 'HarvestTree'
			AND em.Material LIKE '%_log'
			AND e.Timestamp BETWEEN @RangeStart AND @RangeEnd)
		 'HarvestedLogs',

		(SELECT ISNULL(SUM(em.NetAmount), 0)
         FROM EventMaterial em
		 INNER JOIN Event e ON e.Id = em.EventId
		 WHERE e.Type = 'Refuel'
			AND e.Timestamp BETWEEN @RangeStart AND @RangeEnd)
		'FuelConsumed',

		(SELECT
            e.CCNum,
			ISNULL(SUM(IIF(em.NetAmount > 0, em.NetAmount, 0)), 0) 'Gained',
			ISNULL(SUM(IIF(em.NetAmount < 0, em.NetAmount, 0)), 0) 'Lost',
			ISNULL(SUM(em.NetAmount), 0) 'NetAmount',
			(SELECT TOP (1) e2.Timestamp FROM Event e2
			 WHERE e2.CCType = e.CCType
				AND e2.CCNum = e.CCNum
				AND e2.Type = 'HarvestTree'
			 ORDER BY e2.Id DESC)
			'LastTree',
			(SELECT TOP (1) em2.Material FROM EventMaterial em2
			 INNER JOIN Event e2 ON e2.Id = em2.EventId
				AND e2.CCType = e.CCType
				AND e2.CCNum = e.CCNum
				AND e2.Timestamp BETWEEN @RangeStart AND @RangeEnd
			 WHERE em2.Material NOT IN (
                'minecraft:oak_log',
				'minecraft:oak_sapling',
				'minecraft:stick',
                'delightful:acorn')
			 GROUP BY em2.Material
			 ORDER BY SUM(em2.NetAmount) DESC)
			'FavoriteExtra'
		 FROM EventMaterial em
		 INNER JOIN Event e ON e.Id = em.EventId
		 WHERE e.Timestamp BETWEEN @RangeStart AND @RangeEnd
		 GROUP BY e.CCType, e.CCNum
		 ORDER BY SUM(em.NetAmount) DESC
		 FOR JSON PATH)
		'TurtleLeaderboard'

	FOR JSON PATH
END
