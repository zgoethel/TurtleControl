
CREATE VIEW [dbo].[EventNetMaterial] AS
SELECT e.Id,
	e.Type,
	e.Timestamp,
	e.CCType,
	e.CCNum,
	SUM(IIF(em.NetAmount > 0, em.NetAmount, 0)) 'Gained',
	SUM(IIF(em.NetAmount < 0, em.NetAmount, 0)) 'Lost',
	ISNULL(SUM(em.NetAmount), 0) 'NetAmount',
	ISNULL(MAX(em.LastUpdated), e.Timestamp) 'LastUpdated'
FROM Event e
LEFT JOIN EventMaterial em ON e.Id = em.EventId
GROUP BY e.Id, e.Type, e.Timestamp, e.CCType, e.CCNum

--SELECT * FROM EventNetMaterial ORDER BY Id Desc
--SELECT CCNum, Type, Id, Material, NetAmount, Transactions, LastUpdated FROM EventMaterial INNER JOIN Event ON Id = EventId ORDER BY EventId DESC, Material
