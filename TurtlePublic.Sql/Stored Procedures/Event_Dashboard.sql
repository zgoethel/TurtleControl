CREATE PROCEDURE [dbo].[Event_Dashboard]
	@RangeStart DATETIME = NULL,
	@RangeEnd DATETIME = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT @RangeStart = ISNULL(@RangeStart, DATEADD(DAY, -1, GETDATE())),
		@RangeEnd = ISNULL(@RangeEnd, DATEADD(DAY, 1, @RangeStart))
	SELECT
		(SELECT TOP (1) Timestamp FROM Event
		 WHERE Type = 'HarvestTree'
		 ORDER BY Id DESC)
		 'MostRecentTree',

		(SELECT ISNULL(COUNT(*), 0) FROM Event
		 WHERE Type = 'HarvestTree'
			AND Timestamp BETWEEN @RangeStart AND @RangeEnd)
		 'HarvestedTrees',

		(SELECT ISNULL(SUM(NetAmount), 0) FROM EventMaterial em
		 INNER JOIN Event e ON e.Id = em.EventId
		 WHERE Type = 'HarvestTree'
			AND Material LIKE '%_log'
			AND Timestamp BETWEEN @RangeStart AND @RangeEnd)
		 'HarvestedLogs',

		(SELECT ISNULL(SUM(IIF(NetAmount > 0, NetAmount, 0)), 0) FROM EventMaterial em
		 INNER JOIN Event e ON e.Id = em.EventId
		 WHERE e.CCType = 'Turtle'
			AND e.CCNum IN (2, 4)
			AND e.Timestamp BETWEEN @RangeStart AND @RangeEnd)
		 'ItemsCollected',

		(SELECT ISNULL(SUM(NetAmount), 0) FROM EventMaterial em
		 INNER JOIN Event e ON e.Id = em.EventId
		 WHERE Type = 'Refuel'
			AND Timestamp BETWEEN @RangeStart AND @RangeEnd)
		'FuelConsumed',

		(SELECT e.CCNum,
			ISNULL(SUM(IIF(NetAmount > 0, NetAmount, 0)), 0) 'Gained',
			ISNULL(SUM(IIF(NetAmount < 0, NetAmount, 0)), 0) 'Lost',
			ISNULL(SUM(NetAmount), 0) 'NetAmount',
			(SELECT TOP (1) Timestamp FROM Event e2
			 WHERE e2.CCType = e.CCType
				AND e2.CCNum = e.CCNum
				AND Type = 'HarvestTree'
			 ORDER BY Id DESC)
			'LastTree',
			(SELECT TOP (1) Material FROM EventMaterial em2
			 INNER JOIN Event e2 ON e2.Id = em2.EventId
				AND e2.CCType = e.CCType
				AND e2.CCNum = e.CCNum
				AND e2.Timestamp BETWEEN @RangeStart AND @RangeEnd
			 WHERE Material NOT IN ('minecraft:oak_log',
				'minecraft:oak_sapling',
				'minecraft:stick')
			 GROUP BY Material
			 ORDER BY SUM(NetAmount) DESC)
			'FavoriteExtra'
		 FROM EventMaterial em
		 INNER JOIN Event e ON e.Id = em.EventId
		 WHERE e.Timestamp BETWEEN @RangeStart AND @RangeEnd
		 GROUP BY e.CCType, e.CCNum
		 ORDER BY SUM(NetAmount) DESC
		 FOR JSON PATH)
		'TurtleLeaderboard'
	FOR JSON PATH
END
